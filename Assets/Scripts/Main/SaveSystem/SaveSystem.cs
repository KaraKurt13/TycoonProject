using Assets.Scripts.Main.Savesystem;
using Assets.Scripts.Objects;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Main.Save
{
    public class SaveSystem
    {
        private Engine _engine;

        private string _path;

        public SaveSystem(Engine engine)
        {
            _engine = engine;
            _path = Path.Combine(Application.persistentDataPath, "savefile.json");
        }

        public void SaveGame()
        {

            var saveData = new SaveData();
            saveData.CurrencyAmount = _engine.StoreManager.CurrencyAmount;
            saveData.SatisfactionRate = _engine.StoreManager.Satisfaction;

            var storage = _engine.StoreManager.Storage;
            saveData.StoreStorage = new StorageSaveData();
            saveData.StoreStorage.MaxSlots = storage.MaxSlots;
            saveData.StoreStorage.Items = storage.Items;
            saveData.SellPrices = _engine.StoreManager.SellPrices;

            var territories = _engine.Terrain.BuildingTerritories;
            var id = 0;
            foreach (var territory in territories)
            {
                var territoryData = new BuildingTerritoryData();
                territoryData.IsUnlocked = territory.IsUnlocked;
                territoryData.ID = id;
                saveData.TerritoriesData.Add(territoryData);
                id++;
            }

            foreach (var building in _engine.Buildings)
            {
                var data = new BuildingSaveData();
                data.CenterTile = building.InitialTile.CellPosition;
                data.Type = building.Type.Type;
                saveData.BuildingsData.Add(data);
                if (building.Property is ShelfProperty shelf)
                {
                    var storageData = new StorageSaveData();
                    storageData.MaxSlots = shelf.Storage.MaxSlots;
                    storageData.Items = shelf.Storage.Items;
                    data.StorageSaveData = storageData;
                }
            }

            var json = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(_path, json);
            Debug.Log($"Data saved: {_path}");
        }

        public SaveData LoadGame()
        {
            if (File.Exists(_path))
            {
                string json = File.ReadAllText(_path);
                var data = JsonConvert.DeserializeObject<SaveData>(json);
                Debug.Log("Data loaded!");
                return data;
            }
            else
            {
                Debug.LogWarning("Can't find load data.");
                return null;
            }
        }
    }
}