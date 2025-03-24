using Assets.Scripts.Main;
using Assets.Scripts.Terrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            var propertyTemplate = Type.PropertyTemplate;

            if (propertyTemplate == null)
                return;
        }
    }
}