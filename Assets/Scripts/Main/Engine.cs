using Assets.Scripts.Objects;
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
        public GameTerrain Terrain;

        public DataLibrary DataLibrary;

        public SelectionBox SelectionBox;

        public StoreManager StoreManager;

        public ConstructionSystem ConstructionSystem;

        public Building CreateBuilding(GameTile centerTile, BuildingTypeEnum type)
        {
            if (type == BuildingTypeEnum.None)
                return null;

            var buildingCenter = Terrain.GetTilesCenter(centerTile, type, OrientationEnum.E);
            var buildingType = DataLibrary.BuildingTypes[type];
            var building = Instantiate(buildingType.Prefab, buildingCenter, Quaternion.identity).GetComponent<Building>();
            building.Engine = this;
            building.Type = buildingType;
            centerTile.Building = building;
            StoreManager.Buildings.Add(building);
            building.Initialize();
            return building;
        }

        private void Start()
        {
            JsonDataManager.Initialize();
            DataLibrary = new();
            DataLibrary.Initialize();
            Terrain.Initialize();
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
                        if (Input.GetMouseButtonDown(0) && territory.CanBeUnlocked())
                            Terrain.ExpandTerritory(territory);
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            var tile = Terrain.GetTile(hit.point);
                            if (tile != null)
                                SelectionBox.SetSelection(tile.Center, 1, 1);
                        }
                    }
                }
            }
            else
                SelectionBox.ClearSelection();
  
        }
    }
}