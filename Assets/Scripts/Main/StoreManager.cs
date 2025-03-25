using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class StoreManager : MonoBehaviour
    {
        public Engine Engine;

        public int CurrencyAmount { get; private set; }

        public Storage Storage;

        public List<Building> Buildings = new();

        public Dictionary<ItemTypeEnum, int> SellPrices;

        public void Initialize()
        {
            Storage = new Storage(Engine, 24);
            Storage.AddItem(ItemTypeEnum.Sugar, 10);
            Storage.AddItem(ItemTypeEnum.Sugar, 10);
            Storage.AddItem(ItemTypeEnum.Milk, 5);
            Storage.AddItem(ItemTypeEnum.Eggs, 20);
            Storage.AddItem(ItemTypeEnum.Oil, 5);
            SellPrices = new();
            foreach (var item in Engine.DataLibrary.ItemTypes)
                SellPrices.Add(item.Key, item.Value.BuyPrice + 2);

            Engine.CreateBuilding(Engine.Terrain.GetTile(4,3), BuildingTypeEnum.MediumShelf);
            Engine.CreateBuilding(Engine.Terrain.GetTile(11,3), BuildingTypeEnum.MediumShelf);
            Engine.CreateBuilding(Engine.Terrain.GetTile(4, -3), BuildingTypeEnum.CashRegister);
            AddCurrency(200);
        }

        public void AddCurrency(int amount)
        {
            CurrencyAmount += amount;
        }

        public void RemoveCurrency(int amount)
        {
            CurrencyAmount -= amount;
        }

        public void UpdateItemPrice(ItemTypeEnum item, int value)
        {
            SellPrices[item] = value;
        }

        public Building GetRandomRequiredShelf()
        {
            var shelfs = Buildings.Where(b => b.Property is ShelfProperty);
            // Get list of shelfs and check for required product. Return null if there are shelfs with required resource 
            return null;
        }

        public Building GetCashRegister()
        {
            var cashRegisters = Buildings.Where(b => b.Property is CashRegisterProperty);
            // Get list of cash registers and get cash register with least units in queue. If there are no free cash registers,
            // unit will wait until there will be one
            return null;
        }

        public bool CanBuyItem(ItemTypeEnum item, int amount)
        {
            return CurrencyAmount >= GetItemPrice(item, amount) && Storage.CanAddItem(item);
        }

        public void BuyItem(ItemTypeEnum item, int amount)
        {
            if (!CanBuyItem(item, amount))
                return;

            var price = GetItemPrice(item, amount);
            CurrencyAmount -= price;
            Storage.AddItem(item, price);
        }

        public int GetItemPrice(ItemTypeEnum item, int amount)
        {
            var itemType = Engine.DataLibrary.ItemTypes[item];
            return itemType.BuyPrice * amount;
        }
    }
}