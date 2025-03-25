using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PricesManagementComponent : ComponentBase
    {
        [SerializeField] private Transform _itemPricesContainer;
        [SerializeField] private GameObject _itemPriceManagementPrefab;

        protected override void Draw()
        {
            var prices = Engine.StoreManager.SellPrices;
            foreach (var item in prices)
            {
                var itemType = Engine.DataLibrary.ItemTypes[item.Key];
                var itemObject = Instantiate(_itemPriceManagementPrefab, _itemPricesContainer).GetComponent<ItemPriceManagementSubcomponent>();
                itemObject.ItemName.text = itemType.Name;
                itemObject.ItemIcon.sprite = itemType.Icon;
                itemObject.UpdateSellPrice(itemType.BuyPrice, item.Value);
                itemObject.PriceSlider.minValue = itemType.BuyPrice - 10;
                itemObject.PriceSlider.maxValue = itemType.BuyPrice + 10;
                itemObject.PriceSlider.value = item.Value;
                itemObject.PriceSlider.onValueChanged.AddListener(v =>
                {
                    Engine.StoreManager.UpdateItemPrice(item.Key, (int)v);
                    itemObject.UpdateSellPrice(itemType.BuyPrice, (int)v);
                    
                });
            }
        }

        protected override void Refresh()
        {
        }

        protected override void Undraw()
        {
            ClearContainer(_itemPricesContainer);
        }
    }
}