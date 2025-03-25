using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain.Navigation
{
    public class NavigationManager : MonoBehaviour
    {
        public static NavigationManager Instance;

        public Engine Engine;

        private NavigationGrid _grid;

        public void Initialize(List<(Vector3Int cell, Vector3 pos)> cellPositions)
        {
            Instance = this;
            _grid = new NavigationGrid(cellPositions, Engine);
        }

        public void UpdateNode(Vector3Int cell, bool isWalkable)
        {
            _grid.GetNode(cell).IsWalkable = isWalkable;
        }

        public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos)
        {
            Node startNode = _grid.GetNodeAtWorldPosition(startWorldPos);
            Node targetNode = _grid.GetNodeAtWorldPosition(targetWorldPos);

            if (!targetNode.IsWalkable) return null;

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost ||
                        (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbor in currentNode.Neighbors)
                {
                    if (!neighbor.IsWalkable || closedSet.Contains(neighbor)) continue;

                    float newCostToNeighbor = currentNode.GCost + Vector3.Distance(
                        new Vector3(currentNode.Position.x, 0, currentNode.Position.z),
                        new Vector3(neighbor.Position.x, 0, neighbor.Position.z)
                    );

                    if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newCostToNeighbor;
                        neighbor.HCost = Vector3.Distance(
                            new Vector3(neighbor.Position.x, 0, neighbor.Position.z),
                            new Vector3(targetNode.Position.x, 0, targetNode.Position.z)
                        );
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private List<Vector3> RetracePath(Node startNode, Node endNode)
        {
            List<Vector3> path = new List<Vector3>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}