using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class TileEdge
    {
        public GameTile TileA, TileB;

        public GameObject Wall;

        public Vector3 Center;

        public EdgeOrientationEnum Orientation;

        public TileEdge(GameTile tileA, GameTile tileB, EdgeOrientationEnum orientation)
        {
            TileA = tileA;
            TileB = tileB;
            Orientation = orientation;
        }

        public IEnumerable<GameTile> GetTiles()
        {
            yield return TileA;
            yield return TileB;
        }
    }
}