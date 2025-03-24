using Assets.Scripts.Main;
using Assets.Scripts.Terrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.EditorGUI;

namespace Assets.Scripts.Objects
{
    public class Building : MonoBehaviour
    {
        public string Name => Type.Name;

        public string Description => Type.Description;

        public Engine Engine;

        public BuildingType Type { get; set; }

        public BuildingProperty Property;

        public List<GameTile> Tiles = new();

        public void Initialize()
        {
            if (Type.PropertyTemplate != null)
            {
                var propertyCopy = SystemExtensions.Clone(Type.PropertyTemplate);
                propertyCopy.Building = this;
                propertyCopy.Initialize();
                Property = propertyCopy;
            }
        }

        public void Tick()
        {
            Property?.Tick();
        }
    }
}