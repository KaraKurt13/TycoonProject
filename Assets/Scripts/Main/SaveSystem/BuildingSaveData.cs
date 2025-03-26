using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main.Save
{
    public class BuildingSaveData
    {
        public BuildingTypeEnum Type;

        public Vector3Int CenterTile;

        public StorageSaveData StorageSaveData;
    }
}