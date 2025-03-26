using Assets.Scripts.Main;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Terrain
{
    public class GameTerrain : MonoBehaviour
    {
        public Engine Engine;

        public List<BuildingTerritory> BuildingTerritories;

        public BuildingTerritory InitialTerritory;

        public Tilemap BuildingTilemap;

        public TileBase TileBase;

        private Dictionary<Vector3Int, GameTile> _tiles;

        public void Initialize()
        {
            _tiles = new();

            foreach (var territory in BuildingTerritories)
            {
                var center = BuildingTilemap.WorldToCell(territory.gameObject.transform.position);
                var width = territory.Width;
                var height = territory.Height;
                int startX = center.x - width / 2;
                int endX = center.x + width / 2;
                int startY = center.y - height / 2;
                int endY = center.y + height / 2;
                int totalTiles = (width + 1) * (height + 1);
                var tileCenterOffset = new Vector3(0.5f, 0, 0.5f);

                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        var cellPosition = new Vector3Int(x, y, 0);
                        var tileCenter = BuildingTilemap.CellToWorld(cellPosition) + tileCenterOffset;
                        var tile = new GameTile(cellPosition, tileCenter);

                        territory.RelatedTiles.Add(tile);
                        _tiles.Add(cellPosition, tile);
                    }
                }
            }

            foreach (var tile in _tiles.Values)
            {
                if (tile.TopEdge == null)
                {
                    var topTile = GetTile(tile.CellPosition.x, tile.CellPosition.y + 1);
                    var topEdge = new TileEdge(tile, topTile, EdgeOrientationEnum.Vertical);
                    tile.TopEdge = topEdge;
                    if (topTile != null)
                        topTile.BottomEdge = topEdge;
                    topEdge.Center = tile.Center + new Vector3(0, 0, 0.5f);
                }

                if (tile.BottomEdge == null)
                {
                    var bottomTile = GetTile(tile.CellPosition.x, tile.CellPosition.y - 1);
                    var bottomEdge = new TileEdge(tile, bottomTile, EdgeOrientationEnum.Vertical);
                    tile.BottomEdge = bottomEdge;
                    if (bottomTile != null)
                        bottomTile.TopEdge = bottomEdge;
                    bottomEdge.Center = tile.Center + new Vector3(0, 0, -0.5f);
                }

                if (tile.LeftEdge == null)
                {
                    var leftTile = GetTile(tile.CellPosition.x - 1, tile.CellPosition.y);
                    var leftEdge = new TileEdge(tile, leftTile, EdgeOrientationEnum.Horizontal);
                    tile.LeftEdge = leftEdge;
                    if (leftTile != null)
                        leftTile.RightEdge = leftEdge;
                    leftEdge.Center = tile.Center + new Vector3(-0.5f, 0, 0);
                }

                if (tile.RightEdge == null)
                {
                    var rightTile = GetTile(tile.CellPosition.x + 1, tile.CellPosition.y);
                    var rightEdge = new TileEdge(tile, rightTile, EdgeOrientationEnum.Horizontal);
                    tile.RightEdge = rightEdge;
                    if (rightTile != null)
                        rightTile.LeftEdge = rightEdge;
                    rightEdge.Center = tile.Center + new Vector3(0.5f, 0, 0);
                }
            }

            ExpandTerritory(InitialTerritory);
            List<(Vector3Int cell, Vector3 pos)> tileList = _tiles
                .Select(kvp => (kvp.Key, kvp.Value.Center))
                .ToList();
            Engine.NavigationManager.Initialize(tileList);
        }

        /// <summary>
        /// Returns a tile by cell coordinates on a tilemap
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GameTile GetTile(int x, int y, bool onlyUnlocked = false)
        {
            var cellPosition = new Vector3Int(x, y, 0);
            if (_tiles.TryGetValue(cellPosition, out var tile))
            {
                return onlyUnlocked && !tile.IsUnlocked ? null : tile;
            }

            return null;
        }

        /// <summary>
        /// Returns a tile by world position
        /// </summary>
        /// <param name="worldPosition"></param>
        public GameTile GetTile(Vector3 worldPosition, bool onlyUnlocked = false)
        {
            var cellPosition = BuildingTilemap.WorldToCell(worldPosition);
            return GetTile(cellPosition.x, cellPosition.y, onlyUnlocked);
        }

        public void ExpandTerritory(BuildingTerritory territory)
        {
            TileBase[] tileBases = new TileBase[territory.RelatedTiles.Count];
            Vector3Int[] tilePositions = new Vector3Int[territory.RelatedTiles.Count];

            int index = 0;
            foreach (var tile in territory.RelatedTiles)
            {
                tile.IsUnlocked = true;
                tilePositions[index] = tile.CellPosition;
                tileBases[index] = TileBase;
                index++;
            }

            territory.Unlock();
            BuildingTilemap.SetTiles(tilePositions, tileBases);
            UpdateWalls();
        }

        [SerializeField] private GameObject _horizontalWall, _verticalWall;

        public void UpdateWalls()
        {
            var tiles = _tiles.Values.Where(t => t.IsUnlocked);
            foreach (var tile in tiles)
            {
                foreach (var edge in tile.GetEdges())
                {
                    var needsWall = edge.GetTiles().Any(t => t == null || !t.IsUnlocked);
                    var hasWall = edge.Wall != null;

                    if (needsWall && !hasWall)
                    {
                        var prefab = edge.Orientation == EdgeOrientationEnum.Vertical ? _verticalWall : _horizontalWall;
                        var rotation = edge.Orientation == EdgeOrientationEnum.Horizontal
                            ? Quaternion.Euler(0, 90, 0)
                            : Quaternion.identity;

                        edge.Wall = Instantiate(prefab, edge.Center, rotation);
                    }

                    if (!needsWall && hasWall)
                    {
                        Destroy(edge.Wall);
                        edge.Wall = null;
                    }
                }
            }

            // Destroying entry walls
            Destroy(GetTile(7, -5).BottomEdge.Wall);
            Destroy(GetTile(8, -5).BottomEdge.Wall);
            
        }

        public IEnumerable<GameTile> GetOrientationTiles(GameTile centerTile, OrientationEnum orientation, int xSize, int zSize, bool onlyUnlocked = false)
        {
            var x = centerTile.CellPosition.x;
            var y = centerTile.CellPosition.y;

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < zSize; j++)
                {
                    switch (orientation)
                    {
                        case OrientationEnum.E:
                            yield return GetTile(x + i, y + j, onlyUnlocked);
                            break;
                        case OrientationEnum.N:
                            yield return GetTile(x - j, y + i, onlyUnlocked);
                            break;
                        case OrientationEnum.W:
                            yield return GetTile(x - i, y - j, onlyUnlocked);
                            break;
                        case OrientationEnum.S:
                            yield return GetTile(x + j, y - i, onlyUnlocked);
                            break;
                    }
                }
            }
        }

        public Vector3 GetTilesCenter(GameTile tile, BuildingTypeEnum type, OrientationEnum orientation)
        {
            var buildingType = Engine.DataLibrary.BuildingTypes[type];
            var xSize = buildingType.XSize;
            var zSize = buildingType.ZSize;
            var x = tile.CellPosition.x;
            var y = tile.CellPosition.y;

            var vectors = new List<Vector3>();

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < zSize; j++)
                {
                    Vector3Int cellPosition;

                    switch (orientation)
                    {
                        case OrientationEnum.E:
                            cellPosition = new Vector3Int(x + i, y + j, 0);
                            break;
                        case OrientationEnum.N:
                            cellPosition = new Vector3Int(x - j, y + i, 0);
                            break;
                        case OrientationEnum.W:
                            cellPosition = new Vector3Int(x - i, y - j, 0);
                            break;
                        case OrientationEnum.S:
                            cellPosition = new Vector3Int(x + j, y - i, 0);
                            break;
                        default:
                            continue;
                    }

                    vectors.Add(BuildingTilemap.GetCellCenterWorld(cellPosition));
                }

            }

            var sum = Vector3.zero;
            foreach (var vector in vectors)
            {
                sum += vector;
            }

            return sum / vectors.Count;
        }

        public void ResetAll()
        {
            BuildingTilemap.ClearAllTiles();

            foreach (var territory in BuildingTerritories)
                territory.Lock();

            foreach (var tile in _tiles.Values)
            {
                tile.IsUnlocked = false;
                foreach (var edge in tile.GetEdges())
                {
                    Destroy(edge.Wall);
                    edge.Wall = null;
                }
            }
        }
    }
}