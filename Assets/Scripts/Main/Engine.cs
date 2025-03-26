using Assets.Scripts.Helpers;
using Assets.Scripts.Main.Save;
using Assets.Scripts.Objects;
using Assets.Scripts.Terrain;
using Assets.Scripts.Terrain.Navigation;
using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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

        public List<Building> Buildings;

        public List<Customer> Customers;

        public SaveSystem SaveSystem;

        [SerializeField] private GameObject _customerPrefab;

        public Building CreateBuilding(GameTile centerTile, BuildingTypeEnum type, OrientationEnum orientation)
        {
            if (type == BuildingTypeEnum.None)
                return null;

            var buildingCenter = Terrain.GetTilesCenter(centerTile, type, orientation);
            var buildingType = DataLibrary.BuildingTypes[type];
            var tiles = Terrain.GetOrientationTiles(centerTile, orientation, buildingType.XSize, buildingType.ZSize);
            buildingCenter.y = buildingType.Prefab.transform.position.y;
            var building = Instantiate(buildingType.Prefab, buildingCenter, Quaternion.identity).GetComponent<Building>();
            var rotationAngle = GetRotationAngle(orientation);
            building.transform.eulerAngles = new Vector3(0, rotationAngle, 0);
            building.Tiles = tiles.ToList();
            building.InitialTile = centerTile;
            building.Engine = this;
            building.Type = buildingType;
            building.Orientation = orientation;
            foreach (var tile in tiles)
                tile.Building = building;
            Buildings.Add(building);
            building.Initialize();
            return building;
        }

        public Customer CreateCustomer(GameTile tile, bool instantInitialize = true)
        {
            var position = tile.Center;
            position.y = _customerPrefab.transform.position.y;
            var customer = Instantiate(_customerPrefab, position, Quaternion.identity).GetComponent<Customer>();
            customer.Engine = this;
            Customers.Add(customer);
            if (instantInitialize)
                customer.Initialize();
            return customer;
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private void Start()
        {
            JsonDataManager.Initialize();
            DataLibrary = new();
            DataLibrary.Initialize();
            Terrain.Initialize();
            StoreManager.Initialize();
            CreateCustomer(Terrain.GetTile(7, -5));
            SaveSystem = new(this);
            _maxTicksTillAutosave = TimeHelper.SecondsToTicks(30);
            TicksTillAutosave = _maxTicksTillAutosave;
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && !EventSystem.current.IsPointerOverGameObject())
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

        public int TicksTillAutosave;
        private int _maxTicksTillAutosave;

        private void FixedUpdate()
        {
            TicksTillAutosave--;
            if (TicksTillAutosave <= 0)
            {
                Save();
                TicksTillAutosave = _maxTicksTillAutosave;
            }
        }

        public int GetRotationAngle(OrientationEnum orientation)
        {
            return orientation switch
            {
                OrientationEnum.E => 0,
                OrientationEnum.N => 90,
                OrientationEnum.W => 180,
                OrientationEnum.S => 270,
                _ => 0
            };
        }

        #region SaveLoad
        public void Save()
        {
            SaveSystem.SaveGame();
        }

        public void Load()
        {
            ClearCurrentGame();
            var data = SaveSystem.LoadGame();
            StoreManager.CurrencyAmount = data.CurrencyAmount;
            StoreManager.Satisfaction = data.SatisfactionRate;
            StoreManager.Storage = new Storage(this, data.StoreStorage.MaxSlots);
            StoreManager.Storage.Items = data.StoreStorage.Items;
            StoreManager.SellPrices = data.SellPrices;

            foreach (var buildingTerritory in data.TerritoriesData)
            {
                if (buildingTerritory.IsUnlocked)
                {
                    var territory = Terrain.BuildingTerritories[buildingTerritory.ID];
                    Terrain.ExpandTerritory(territory);
                }    
            }

            foreach (var buildingData in data.BuildingsData)
            {
                var tile = Terrain.GetTile(buildingData.CenterTile.x, buildingData.CenterTile.y);
                var building = CreateBuilding(tile, buildingData.Type, buildingData.Orientation);
                if (building.Property is ShelfProperty shelf)
                {
                    var storageData = buildingData.StorageSaveData;
                    shelf.Storage = new Storage(this, storageData.MaxSlots);
                    shelf.Storage.Items = storageData.Items;
                }
            }

            foreach (var customerData in data.CustomersData)
            {
                var tile = StoreManager.ExitTile;
                var customer = CreateCustomer(tile, false);
                var position = new Vector3(customerData.X, customerData.Y, customerData.Z);
                customer.transform.position = position;
                customer.PurchasedItems = customerData.PurchasedItems;
                customer.PurchaseList = customerData.PurchaseList;
                customer.Satisfaction = customerData.Satisfaction;
                customer.Initialize(false);
            }

            TicksTillAutosave = data.TicksTillAutosave;
            StoreManager.TickTillCustomerRespawn = data.TicksTillCustomerRespawn;
        }

        private void ClearCurrentGame()
        {
            foreach (var customer in Customers.ToList())
                customer.Destroy();

            foreach (var building in Buildings.ToList())
                building.Destroy();

            Terrain.ResetAll();
        }
        #endregion SaveLoad
    }
}