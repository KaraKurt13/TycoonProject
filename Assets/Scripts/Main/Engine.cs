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
                    SelectionBox.SetSelection(territory.Center, territory.Width, territory.Height);
                }
            }
            else
                SelectionBox.ClearSelection();
        }

        public void CreateBuilding()
        {

        }

        public void ExpandTerritory(BuildingTerritory territory)
        {
            TileBase[] tileBases = new TileBase[territory.RelatedTiles.Count];
            Vector3Int[] tilePositions = new Vector3Int[territory.RelatedTiles.Count];

            int index = 0;
            foreach (var tile in territory.RelatedTiles)
            {
                tile.IsUnlocked = true;
                tilePositions[index] = new Vector3Int(tile.X, tile.Y, 0);
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
                Vector3Int[] positions = new Vector3Int[totalTiles];

                int index = 0;
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        var tile = new GameTile();
                        tile.X = x;
                        tile.Y = y;
                        territory.RelatedTiles.Add(tile);
                        index++;
                    }
                }
            }
        }
    }
}