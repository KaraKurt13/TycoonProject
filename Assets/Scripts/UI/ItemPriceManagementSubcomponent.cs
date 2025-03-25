using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace Assets.Scripts.UI
{
    public class ItemPriceManagementSubcomponent : MonoBehaviour
    {
        public Image ItemIcon;

        public TextMeshProUGUI BuyPrice, SellPrice, ItemName;

        public Slider PriceSlider;

        public void UpdateSellPrice(int buyPrice, int sellPrice)
        {
            BuyPrice.text = $"{buyPrice}$";
            SellPrice.text = $"{sellPrice}$";
            var profitColor = buyPrice < sellPrice ? Color.green : Color.red;
            SellPrice.color = profitColor;
        }
    }
}