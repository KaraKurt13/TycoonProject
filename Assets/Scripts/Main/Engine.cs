using Assets.Scripts.Objects;
using Assets.Scripts.Terrain;
using Assets.Scripts.Terrain.Navigation;
using Assets.Scripts.UI;
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

        public ComponentsController ComponentsController;

        public NavigationManager NavigationManager;

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

        [SerializeField] private GameObject _customerPrefab;

        public void CreateCustomer(GameTile tile)
        {
            var customer = Instantiate(_customerPrefab, tile.Center, Quaternion.identity).GetComponent<Customer>();
            var t = Terrain.GetTile(8, -5);
            customer.Agent.SetDestination(StoreManager.GetRandomRequiredShelf().transform.position);
        }

        private void Start()
        {
            JsonDataManager.Initialize();
            DataLibrary = new();
            DataLibrary.Initialize();
            Terrain.Initialize();
            StoreManager.Initialize();
            CreateCustomer(Terrain.GetTile(7, -5));
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<BuildingTerritory>(out var territory))
                {
                    if (!territory.IsUnlocked)
                    {
                        SelectionBox.SetSelection(territory.Center, territory.Width + 1, territory.Height + 1);
                        if (Input.GetMouseButtonDown(0) && territory.CanBeUnlocked())
                            ComponentsController.TerritoryPurchaseComponent.DrawPurchaseInfo(territory);
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            var tile = Terrain.GetTile(hit.point);
                            if (tile != null)
                            {
                                SelectionBox.SetSelection(tile.Center, 1, 1);
                                Debug.Log(tile.CellPosition);
                            }
                        }
                    }
                }

                if (Input.GetMouseButtonDown(0) && hit.collider.TryGetComponent<Building>(out var building))
                {
                    ComponentsController.DrawBuildingManagement(building);
                }
            }
            else
                SelectionBox.ClearSelection();

        }
    }
}