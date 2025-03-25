using Assets.Scripts.Main;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ShelfManagementComponent : MonoBehaviour
    {
        public Engine Engine;

        private ShelfProperty _selectedShelf;

        public void DrawShelf(ShelfProperty shelfProperty)
        {
            Hide();
            _selectedShelf = shelfProperty;
            RedrawShelfStorage();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _selectedShelf = null;
            ClearContainer(_storageContainer);
            HideItemSelection();
        }

        [SerializeField] private StorageSlotSubcomponent _slotPrefab;
        [SerializeField] private Transform _storageContainer;
        [SerializeField] private TextMeshProUGUI _shelfName;

        private void RedrawShelfStorage()
        {
            ClearContainer(_storageContainer);
            var storage = _selectedShelf.Storage;
            var index = 0;

            foreach (var item in storage.GetItems())
            {
                var slot = Instantiate(_slotPrefab, _storageContainer).GetComponent<StorageSlotSubcomponent>();
                slot.Icon.sprite = item.Key.Icon;
                slot.AmountText.text = item.Value.ToString();
                slot.OverlayButton.onClick.AddListener(() =>
                {
                    DrawItemSelection();
                });
                index++;
            }

            for (int i = index; i < storage.MaxSlots; i++)
            {
                var slot = Instantiate(_slotPrefab, _storageContainer).GetComponent<StorageSlotSubcomponent>();
                slot.Icon.enabled = false;
                slot.AmountText.text = string.Empty;
                slot.OverlayButton.onClick.AddListener(() =>
                {
                    DrawItemSelection();
                });

            }
            _shelfName.text = _selectedShelf.Building.Name;
        }

        [SerializeField] private GameObject _itemSelectionContent;
        [SerializeField] private Transform _itemsSelectionContainer;

        private void DrawItemSelection()
        {
            _itemSelectionContent.SetActive(true);
            ClearContainer(_itemsSelectionContainer);
            foreach (var item in Engine.StoreManager.Storage.GetItems())
            {
                var itemObject = Instantiate(_slotPrefab, _itemsSelectionContainer).GetComponent<StorageSlotSubcomponent>();
                itemObject.Icon.sprite = item.Key.Icon;
                itemObject.AmountText.text = item.Value.ToString();
                itemObject.OverlayButton.onClick.AddListener(() =>
                {
                    if (!_selectedShelf.Storage.CanAddItem(item.Key.Type))
                        return;
                    _selectedShelf.Storage.AddItem(item.Key.Type, 1);
                    Engine.StoreManager.Storage.TakeItem(item.Key.Type, 1);
                    RedrawShelfStorage();
                    DrawItemSelection();
                });
            }
        }

        private void HideItemSelection()
        {
            _itemSelectionContent.SetActive(false);
            ClearContainer(_itemsSelectionContainer);
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);
        }
    }
}