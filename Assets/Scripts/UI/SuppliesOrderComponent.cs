using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SuppliesOrderComponent : ComponentBase
    {
        [SerializeField] private Transform _suppliesContainer;
        [SerializeField] private GameObject _supplyPrefab;

        protected override void Draw()
        {
            RedrawOrderList();
        }

        protected override void Refresh()
        {

        }

        protected override void Undraw()
        {
            ClearContainer(_suppliesContainer);
        }

        public void RedrawOrderList()
        {
            ClearContainer(_suppliesContainer);
            var items = Engine.DataLibrary.ItemTypes;

            foreach (var item in items.Values)
            {
                var itemObject = Instantiate(_supplyPrefab, _suppliesContainer).GetComponent<SupplyToOrderSubcomponent>();
                itemObject.ItemIcon.sprite = item.Icon;
                itemObject.ItemName.text = item.Name;
                itemObject.BuyPrice.text = item.BuyPrice.ToString();
                itemObject.UpdateAmount(item.Type, 1, Engine);
                itemObject.AmountSlider.onValueChanged.AddListener(v =>
                {
                    itemObject.UpdateAmount(item.Type, (int)v, Engine);
                });
            }
        }


    }
}