using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class StorageComponent : ComponentBase
    {
        [SerializeField] private Transform _slotsContainer;
        [SerializeField] private TextMeshProUGUI _slotsCount;
        [SerializeField] private GameObject _slotPrefab;

        protected override void Draw()
        {
            var storage = Engine.StoreManager.Storage;
            var items = storage.GetItems();
            foreach (var item in items)
            {
                var itemObject = Instantiate(_slotPrefab, _slotsContainer).GetComponent<StorageSlotSubcomponent>();
                itemObject.AmountText.text = item.Value.ToString();
                itemObject.Icon.sprite = item.Key.Icon;
            }
            _slotsCount.text = $"{items.Count()}/{storage.MaxSlots}";
        }

        protected override void Refresh()
        {
        }

        protected override void Undraw()
        {
            ClearContainer(_slotsContainer);
        }
    }
}