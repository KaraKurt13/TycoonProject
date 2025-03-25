using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class BuildingTerritory : MonoBehaviour
    {
        public int Width, Height;

        public Vector3 Center;

        public BoxCollider SelectionCollider;

        public bool IsUnlocked = false;

        public List<GameTile> RelatedTiles = new();

        public List<BuildingTerritory> RequiredTerritories;

        public int Cost;

        public bool CanBeUnlocked()
        {
            return RequiredTerritories.All(t => t.IsUnlocked);
        }

        public void Unlock()
        {
            IsUnlocked = true;
        }

        private void Start()
        {
            SelectionCollider.size = new Vector3(Width + 1, 0, Height + 1);
            Center = transform.position;
        }
    }
}