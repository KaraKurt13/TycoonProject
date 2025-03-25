using Assets.Scripts.Main;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SupplyToOrderSubcomponent : MonoBehaviour
    {
        public TextMeshProUGUI ItemName, BuyPrice, SetAmount;

        public Image ItemIcon;

        public Button BuyButton;

        public Slider AmountSlider;

        public void UpdateAmount(ItemTypeEnum item, int amount, Engine engine)
        {
            var storeManager = engine.StoreManager;
            var canAfford = storeManager.CanBuyItem(item, amount);
            var totalPrice = storeManager.GetItemPrice(item, amount);
            var color = canAfford ? Color.white : Color.red;
            SetAmount.text = amount.ToString();
            BuyPrice.text = totalPrice.ToString();
            BuyPrice.color = color;
            BuyButton.interactable = canAfford;
            BuyButton.onClick.RemoveAllListeners();
            BuyButton.onClick.AddListener(() => 
            { 
                storeManager.BuyItem(item, amount);
                engine.ComponentsController.SuppliesOrderComponent.RedrawOrderList();
            });
        }
    }
}