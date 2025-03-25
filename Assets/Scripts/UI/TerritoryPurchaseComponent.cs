using Assets.Scripts.Main;
using Assets.Scripts.Terrain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TerritoryPurchaseComponent : MonoBehaviour
    {
        public Engine Engine;

        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Button _purchaseButton;

        private BuildingTerritory _selectedTerritory;

        public void DrawPurchaseInfo(BuildingTerritory territory)
        {
            _selectedTerritory = territory;
            UpdatePurchaseStatus();
            _purchaseButton.onClick.AddListener(() =>
            {
                Engine.Terrain.ExpandTerritory(territory);
                Engine.StoreManager.RemoveCurrency(territory.Cost);
                Hide();
            });
            gameObject.SetActive(true);
        }

        private void Update()
        {
            UpdatePurchaseStatus();   
        }

        public void Hide()
        {
            _selectedTerritory = null;
            gameObject.SetActive(false);
        }

        private void UpdatePurchaseStatus()
        {
            var canPurchase = Engine.StoreManager.CurrencyAmount >= _selectedTerritory.Cost;
            var color = canPurchase ? Color.white : Color.red;
            _purchaseButton.interactable = canPurchase;
            _costText.color = color;
            _costText.text = $"{_selectedTerritory.Cost}$";
        }
    }
}