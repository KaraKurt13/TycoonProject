using Assets.Scripts.Objects;
using Assets.Scripts.Terrain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Assets.Scripts.Main
{
    public class ConstructionSystem : MonoBehaviour
    {
        public Engine Engine;

        public bool IsConstructing;

        private GameTile _currentTile;

        private BuildingType _selectedBuilding;

        private void Start()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (!IsConstructing)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var tile = Engine.Terrain.GetTile(hit.point, true);
                if (tile != null)
                    _currentTile = tile;
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && _currentTile != null && ConstructionIsPossible())
            {
                ConstructBuilding();
                StopConstructing();
                return;
            }

            if (_currentTile != null)
                UpdateBlueprint();
        }

        public GameObject Blueprint;
        public MeshRenderer BlueprintRenderer;
        private MaterialPropertyBlock _propertyBlock;

        public void ConstructBuilding()
        {
            if (_currentTile.Building == null)
                Engine.CreateBuilding(_currentTile, _selectedBuilding.Type);
        }

        public void StartConstructing(BuildingTypeEnum type)
        {
            HideConstructionBlueprint();
            DrawConstructionBlueprint(type);
            IsConstructing = true;
        }

        public void StopConstructing()
        {
            HideConstructionBlueprint();
            _selectedBuilding = null;
            IsConstructing = false;
        }

        private void UpdateBlueprint()
        {
            UpdateBlueprintPosition();
            UpdateBlueprintColor();
        }

        private void DrawConstructionBlueprint(BuildingTypeEnum typeEnum)
        {
            _selectedBuilding = Engine.DataLibrary.BuildingTypes[typeEnum];
            var type = Engine.DataLibrary.BuildingTypes[typeEnum];
            var prefab = type.Prefab;
            Blueprint = Instantiate(prefab);
            BlueprintRenderer = Blueprint.GetComponent<MeshRenderer>();
            BlueprintRenderer.GetPropertyBlock(_propertyBlock);
        }

        private void HideConstructionBlueprint()
        {
            Destroy(Blueprint);
            BlueprintRenderer = null;
        }

        private void UpdateBlueprintColor()
        {
            BlueprintRenderer.GetPropertyBlock(_propertyBlock);
            var color = _propertyBlock.GetColor("_Color");
            color = ConstructionIsPossible() ? Color.green : Color.red;
            _propertyBlock.SetColor("_Color", color);
            BlueprintRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void UpdateBlueprintPosition()
        {
            if (Engine.Terrain.GetOrientationTiles(_currentTile, OrientationEnum.E, _selectedBuilding.XSize, _selectedBuilding.ZSize, true).All(t => t != null))
                Blueprint.transform.position = Engine.Terrain.GetTilesCenter(_currentTile, _selectedBuilding.Type, OrientationEnum.E);
        }

        private bool ConstructionIsPossible()
        {
            var occupiedTiles = Engine.Terrain.GetOrientationTiles(_currentTile, OrientationEnum.E, _selectedBuilding.XSize, _selectedBuilding.ZSize, true);
            return occupiedTiles.All(t => t != null && t.Building == null);
        }
    }
}