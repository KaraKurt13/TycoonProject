using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
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

            ExpandTerritory(InitialTerritory);
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
                if (onlyUnlocked)
                    return tile.IsUnlocked ? tile : null;
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
        }
    }
}