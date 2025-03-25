using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class ItemType
    {
        #region GameData
        public string Name { get; set; }

        public ItemTypeEnum Type { get; set; }

        public int BuyPrice { get; set; }
        #endregion GameData

        public Sprite Icon { get; set; }
    }
}
