using Assets.Scripts.Main.Save;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main.Savesystem
{
    public class SaveData
    {
        public int CurrencyAmount, SatisfactionRate;

        public int TicksTillAutosave, TicksTillCustomerRespawn;

        public StorageSaveData StoreStorage;

        public Dictionary<ItemTypeEnum, int> SellPrices;

        public List<CustomerSaveData> CustomersData = new();

        public List<BuildingSaveData> BuildingsData = new();

        public List<BuildingTerritoryData> TerritoriesData = new();
    }
}