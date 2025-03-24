using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class ShelfProperty : BuildingProperty
    {
        public override BuildingPropertyTypeEnum PropertyType => BuildingPropertyTypeEnum.ShelfProperty;

        public int SlotsCount { get; set; }
    }
}