using Assets.Scripts.Terrain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

namespace Assets.Scripts.Main
{
    public class Engine : MonoBehaviour
    {
        public List<BuildingTerritory> BuildingTerritories;

        public BuildingTerritory InitialTerritory;

        public Tilemap BuildingTilemap;

        public TileBase TileBase;

        public SelectionBox SelectionBox;

        public Dictionary<Vector3Int, GameTile> Tiles = new();

        private void Start()
        {
            InitializeTerrain();
            ExpandTerritory(InitialTerritory);
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent<BuildingTerritory>(out var territory))
                {
                    if (!territory.IsUnlocked)
                    {
                        SelectionBox.SetSelection(territory.Center, territory.Width + 1, territory.Height + 1);
                        if (Input.GetMouseButtonDown(0))
                            ExpandTerritory(territory);
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Vector3Int cellPosition = BuildingTilemap.WorldToCell(hit.point);
                            if (Tiles.TryGetValue(cellPosition, out var tile))
                            {
                                SelectionBox.SetSelection(tile.Center, 1, 1);
                            }
                        }
                    }
                }
            }
            else
                SelectionBox.ClearSelection();
  
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

        private void InitializeTerrain()
        {
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
                        Tiles.Add(cellPosition, tile);
                    }
                }
            }
        }
    }
}