using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class GameTile
    {
        public Building Building;

        public TileEdge TopEdge, BottomEdge, LeftEdge, RightEdge;

        public Vector3Int CellPosition;

        public Vector3 Center;

        public bool IsUnlocked = false;

        public GameTile(Vector3Int cell, Vector3 center)
        {
            CellPosition = cell;
            Center = center;
        }

        public IEnumerable<TileEdge> GetEdges()
        {
            yield return TopEdge;
            yield return BottomEdge;
            yield return LeftEdge;
            yield return RightEdge;
        }
    }
}