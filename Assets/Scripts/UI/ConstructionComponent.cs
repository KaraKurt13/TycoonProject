using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ConstructionComponent : ComponentBase
    {
        [SerializeField] private Transform _container;
        [SerializeField] private GameObject _constructionPrefab;

        protected override void Draw()
        {
            RedrawConstructionList();
        }

        protected override void Refresh()
        {
        }

        protected override void Undraw()
        {
            ClearContainer(_container);
        }

        private void RedrawConstructionList()
        {
            var constructions = Engine.DataLibrary.BuildingTypes.Values;
            foreach (var construction in constructions)
            {
                var constructionSettings = Instantiate(_constructionPrefab, _container).GetComponent<BuildingToConstructSubcomponent>();
                constructionSettings.Name.text = construction.Name;
                constructionSettings.Icon.sprite = construction.Icon;
                constructionSettings.Price.text = $"{construction.Cost}$";
                constructionSettings.OverlayButton.onClick.AddListener(() => Engine.ConstructionSystem.StartConstructing(construction.Type));
            }
        }
    }
}