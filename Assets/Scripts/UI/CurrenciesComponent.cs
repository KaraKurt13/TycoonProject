using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CurrenciesComponent : ComponentBase
    {
        [SerializeField] TextMeshProUGUI _mainCurrencyText;

        private void Update()
        {
            Refresh();
        }

        protected override void Draw()
        {
        }

        protected override void Refresh()
        {
            _mainCurrencyText.text = $"{Engine.StoreManager.CurrencyAmount}$";
        }

        protected override void Undraw()
        {
        }
    }
}