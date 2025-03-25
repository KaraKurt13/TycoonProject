using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain.Navigation
{
    public class Node
    {
        public Vector3 Position;

        public Vector3Int GridPosition;

        public bool IsWalkable; // Later replace with some enum (for doors, portals, etc.)

        public float GCost;

        public float HCost;

        public float FCost => GCost + HCost;

        public Node Parent;

        public List<Node> Neighbors;

        public Node(Vector3 position, Vector3Int gridPosition, bool isWalkable)
        {
            Position = position;
            GridPosition = gridPosition;
            IsWalkable = isWalkable;
        }

        public void InitializeNeighbors(NavigationGrid grid)
        {
            Neighbors = new();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    var gridPos = GridPosition + new Vector3Int(dx, dy, 0);

                    if (grid.GetNode(gridPos) != null)
                    {
                        Neighbors.Add(grid.GetNode(gridPos));
                    }
                }
            }
        }
    }
}