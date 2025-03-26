using Assets.Scripts.Objects;
using Assets.Scripts.Terrain;
using System;
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

        private OrientationEnum _constructionOrientation;

        private void Start()
        {
        }

        private void Update()
        {
            if (!IsConstructing)
                return;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _constructionOrientation--;
                if (_constructionOrientation < 0)
                    _constructionOrientation = (OrientationEnum)Enum.GetValues(typeof(OrientationEnum)).Length - 1;
                UpdateBlueprint();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _constructionOrientation++;
                if (_constructionOrientation >= (OrientationEnum)Enum.GetValues(typeof(OrientationEnum)).Length)
                    _constructionOrientation = 0;
                UpdateBlueprint();
            }

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
        public List<MeshRenderer> BlueprintRenderers;

        public void ConstructBuilding()
        {
            if (_currentTile.Building == null)
                Engine.CreateBuilding(_currentTile, _selectedBuilding.Type, _constructionOrientation);
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
            _constructionOrientation = OrientationEnum.E;
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
            var angle = Engine.GetRotationAngle(_constructionOrientation);
            Blueprint = Instantiate(prefab);
            Blueprint.transform.eulerAngles = new Vector3(0, angle, 0);
            BlueprintRenderers = Blueprint.GetComponent<Building>().Renderers;
        }

        private void HideConstructionBlueprint()
        {
            Destroy(Blueprint);
            BlueprintRenderers.Clear();
        }

        private void UpdateBlueprintColor()
        {
            var color = ConstructionIsPossible() ? Color.green : Color.red;
            foreach (var renderer in BlueprintRenderers)
                renderer.material.color = color;
        }

        private void UpdateBlueprintPosition()
        {
            if (Engine.Terrain.GetOrientationTiles(_currentTile, _constructionOrientation, _selectedBuilding.XSize, _selectedBuilding.ZSize, true).All(t => t != null))
            {
                var pos = Engine.Terrain.GetTilesCenter(_currentTile, _selectedBuilding.Type, _constructionOrientation);
                var newPos = new Vector3(pos.x, Blueprint.transform.position.y, pos.z);
                Blueprint.transform.position = newPos;
                var angle = Engine.GetRotationAngle(_constructionOrientation);
                Blueprint.transform.eulerAngles = new Vector3(0, angle, 0);
            }
        }

        private bool ConstructionIsPossible()
        {
            var occupiedTiles = Engine.Terrain.GetOrientationTiles(_currentTile, OrientationEnum.E, _selectedBuilding.XSize, _selectedBuilding.ZSize, true);
            return occupiedTiles.All(t => t != null && t.Building == null) && Engine.StoreManager.CurrencyAmount >= _selectedBuilding.Cost;
        }
    }
}