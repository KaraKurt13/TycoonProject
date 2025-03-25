using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class ShelfProperty : BuildingProperty
    {
        public override BuildingPropertyTypeEnum PropertyType => BuildingPropertyTypeEnum.ShelfProperty;

        public Storage Storage;

        public int SlotsCount { get; set; }

        public override void Initialize(Engine engine)
        {
            Storage = new Storage(engine, SlotsCount);
        }
    }
}