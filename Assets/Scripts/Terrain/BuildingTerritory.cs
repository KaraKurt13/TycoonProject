using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class BuildingTerritory : MonoBehaviour
    {
        public int Width, Height;

        public BoxCollider SelectionCollider;

        public bool IsUnlocked = false;

        public List<GameTile> RelatedTiles = new();

        public void Unlock()
        {
            IsUnlocked = true;
            gameObject.SetActive(false);
        }

        private void Start()
        {
            SelectionCollider.size = new Vector3(Width + 1, 1, Height + 1);
        }
    }
}