using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CurrenciesComponent : ComponentBase
    {
        [SerializeField] TextMeshProUGUI _mainCurrencyText, _satisfactionText;

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
            var satisfaction = Engine.StoreManager.Satisfaction;
            var color = satisfaction >= 0 ? Color.green : Color.red;
            var happy = satisfaction >= 0 ? ":)" : ":(";
            _satisfactionText.text = $"{satisfaction} {happy}";
            _satisfactionText.color = color;
        }

        protected override void Undraw()
        {
        }
    }
}