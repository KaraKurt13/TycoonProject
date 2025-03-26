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

        public GameTile InitialTile;

        public List<GameTile> Tiles = new();

        public List<MeshRenderer> Renderers;

        public void Initialize()
        {
            if (Type.PropertyTemplate != null)
            {
                var propertyCopy = SystemExtensions.Clone(Type.PropertyTemplate);
                propertyCopy.Building = this;
                propertyCopy.Initialize(Engine);
                Property = propertyCopy;
            }
        }

        public void Destroy()
        {
            Engine.Buildings.Remove(this);
            foreach (var tile in Tiles)
                tile.Building = null;

            Destroy(this.gameObject);
        }
    }
}