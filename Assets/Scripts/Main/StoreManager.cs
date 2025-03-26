using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using Assets.Scripts.Terrain;
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

        public int Satisfaction { get; private set; }

        public Storage Storage;

        public Dictionary<ItemTypeEnum, int> SellPrices;

        public GameTile ExitTile;

        private int _ticksTillCustomerSpawn, _maxTicksTillCustomerSpawn;

        private void FixedUpdate()
        {
            _ticksTillCustomerSpawn--;
            if (_ticksTillCustomerSpawn <= 0)
            {
                Engine.CreateCustomer(ExitTile);
                _ticksTillCustomerSpawn = _maxTicksTillCustomerSpawn;
            }
        }

        public void Initialize()
        {
            Storage = new Storage(Engine, 24);
            ExitTile = Engine.Terrain.GetTile(7, -5);
            Storage.AddItem(ItemTypeEnum.Sugar, 10);
            Storage.AddItem(ItemTypeEnum.Sugar, 10);
            Storage.AddItem(ItemTypeEnum.Milk, 5);
            Storage.AddItem(ItemTypeEnum.Eggs, 20);
            Storage.AddItem(ItemTypeEnum.Oil, 5);
            SellPrices = new();
            foreach (var item in Engine.DataLibrary.ItemTypes)
                SellPrices.Add(item.Key, item.Value.BuyPrice + 2);

            var shelf = Engine.CreateBuilding(Engine.Terrain.GetTile(4,3), BuildingTypeEnum.MediumShelf).Property as ShelfProperty;
            shelf.Storage.AddItem(ItemTypeEnum.Sugar, 2);
            shelf.Storage.AddItem(ItemTypeEnum.Salt, 2);
            shelf.Storage.AddItem(ItemTypeEnum.Oil, 1);
            Engine.CreateBuilding(Engine.Terrain.GetTile(11,3), BuildingTypeEnum.MediumShelf);
            Engine.CreateBuilding(Engine.Terrain.GetTile(4, -3), BuildingTypeEnum.CashRegister);
            AddCurrency(500);
            _maxTicksTillCustomerSpawn = TimeHelper.SecondsToTicks(5f);
            _ticksTillCustomerSpawn = _maxTicksTillCustomerSpawn;
        }

        public void AddCurrency(int amount)
        {
            CurrencyAmount += amount;
        }

        public void RemoveCurrency(int amount)
        {
            CurrencyAmount -= amount;
        }

        public void UpdateSatisfaction(int value)
        {
            Satisfaction += value;
        }

        public void UpdateItemPrice(ItemTypeEnum item, int value)
        {
            SellPrices[item] = value;
        }

        public Building GetRandomRequiredShelf(ItemTypeEnum item)
        {
            var shelfs = Engine.Buildings.Where(b => b.Property is ShelfProperty shelf && shelf.Storage.HasItem(item, 1));
            return shelfs.FirstOrDefault();
        }

        public Building GetCashRegister()
        {
            var cashRegisters = Engine.Buildings.Where(b => b.Property is CashRegisterProperty);
            return cashRegisters.FirstOrDefault();
        }

        public bool CanBuyItem(ItemTypeEnum item, int amount)
        {
            return CurrencyAmount >= GetItemBuyPrice(item, amount) && Storage.CanAddItem(item);
        }

        public void BuyItem(ItemTypeEnum item, int amount)
        {
            if (!CanBuyItem(item, amount))
                return;

            var price = GetItemBuyPrice(item, amount);
            CurrencyAmount -= price;
            Storage.AddItem(item, amount);
        }

        public int GetItemBuyPrice(ItemTypeEnum item, int amount)
        {
            var itemType = Engine.DataLibrary.ItemTypes[item];
            return itemType.BuyPrice * amount;
        }

        public int GetItemSellPrice(ItemTypeEnum item, int amount)
        {
            return SellPrices[item] * amount;
        }
    }
}