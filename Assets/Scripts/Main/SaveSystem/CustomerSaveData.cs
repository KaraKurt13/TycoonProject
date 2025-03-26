using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main.Save
{
    public class CustomerSaveData
    {
        public List<ItemTypeEnum> PurchaseList = new();

        public List<ItemTypeEnum> PurchasedItems = new();

        public float X, Y, Z;

        public int Satisfaction = 0;
    }
}