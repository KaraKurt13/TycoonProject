using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class Storage
    {
        public Engine Engine;

        public Dictionary<ItemTypeEnum, int> Items { get; set; } = new();

        public int MaxSlots { get; }

        public Storage(Engine engine, int slotsCount)
        {
            Engine = engine;
            MaxSlots = slotsCount;
        }

        public void AddItem(ItemTypeEnum type, int amount)
        {
            if (!CanAddItem(type))
                return;

            if (Items.ContainsKey(type))
                Items[type] += amount;
            else
                Items[type] = amount;
        }

        public bool HasItem(ItemTypeEnum type, int amount)
        {
            return Items.ContainsKey(type) && Items[type] >= amount;
        }

        public bool CanAddItem(ItemTypeEnum type)
        {
            return Items.ContainsKey(type) || Items.Count < MaxSlots;
        }

        public int GetItemAmount(ItemTypeEnum type)
        {
            return Items[type];
        }

        public bool TakeItem(ItemTypeEnum type, int amount)
        {
            if (!HasItem(type, amount))
                return false;

            Items[type] -= amount;
            if (Items[type] <= 0)
                Items.Remove(type);

            return true;
        }

        public IEnumerable<KeyValuePair<ItemType, float>> GetItems()
        {
            foreach (var item in Items)
            {
                var itemType = Engine.DataLibrary.ItemTypes[item.Key];
                yield return new KeyValuePair<ItemType, float>(itemType, item.Value);
            }
        }
    }
}