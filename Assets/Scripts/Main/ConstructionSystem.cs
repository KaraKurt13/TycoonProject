using Assets.Scripts.Terrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class ConstructionSystem : MonoBehaviour
    {
        public Engine Engine;

        private bool _isConstructing;

        private GameTile _currentTile;

        private void Start()
        {
            _propertyBlock = new MaterialPropertyBlock();
            BlueprintRenderer.material.renderQueue = 3001;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                _isConstructing = !_isConstructing;
                if (!_isConstructing)
                    HideConstructionBlueprint();
                else
                    DrawConstructionBlueprint();
            }

            if (!_isConstructing)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var tile = Engine.Terrain.GetTile(hit.point, true);
                if (tile != null)
                    _currentTile = tile;
            }

            if (Input.GetMouseButtonDown(0) && _currentTile != null)
                ConstructBuilding();

            if (_currentTile != null)
                UpdateBlueprint();
        }

        public GameObject Blueprint;
        public MeshRenderer BlueprintRenderer;
        private MaterialPropertyBlock _propertyBlock;

        public void ConstructBuilding()
        {
            if (_currentTile.Building == null)
                Engine.CreateBuilding(_currentTile);
        }

        public void UpdateBlueprint()
        {
            UpdateBlueprintPosition();
            UpdateBlueprintColor();
        }

        private void UpdateBlueprintColor()
        {
            BlueprintRenderer.GetPropertyBlock(_propertyBlock);
            var color = _propertyBlock.GetColor("_Color");
            color = Color.green;
            _propertyBlock.SetColor("_Color", color);
            BlueprintRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void UpdateConstructionAllowance()
        {

        }

        private void UpdateBlueprintPosition()
        {
            Blueprint.transform.position = _currentTile.Center;
        }

        private void DrawConstructionBlueprint()
        {
            Blueprint.gameObject.SetActive(true);
        }

        private void HideConstructionBlueprint()
        {
            Blueprint.gameObject.SetActive(false);
        }



    }
}