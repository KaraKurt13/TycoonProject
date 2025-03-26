using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main.Save
{
    public class StorageSaveData
    {
        public int MaxSlots { get; set; }

        public Dictionary<ItemTypeEnum, int> Items { get; set; }
    }
}