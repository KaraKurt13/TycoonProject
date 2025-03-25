using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain.Navigation
{
    public class NavigationGrid
    {
        private Dictionary<Vector3Int, Node> _nodes = new();

        public NavigationGrid(List<(Vector3Int cell, Vector3 pos)> tiles, Engine engine)
        {
            foreach (var tile in tiles)
            {
                _nodes[tile.cell] = new Node(tile.pos, tile.cell, true);
            }
            foreach (var node in _nodes)
                node.Value.InitializeNeighbors(this);

            Debug.Log(_nodes.Count);
        }

        public Node GetNode(Vector3Int cell)
        {
            if (_nodes.TryGetValue(cell, out Node node)) 
                return node;

            return null;
        }

        public Node GetNodeAtWorldPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x);
            int y = Mathf.FloorToInt(worldPosition.z);
            var gridPos = new Vector3Int(x, y);
            return _nodes[gridPos];
        }
    }
}