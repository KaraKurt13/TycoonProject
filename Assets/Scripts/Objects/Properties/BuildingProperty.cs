using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public abstract class BuildingProperty
    {
        public Building Building;

        public abstract BuildingPropertyTypeEnum PropertyType { get; }

        public virtual void Tick() { }

        public virtual void Initialize() { }
    }
}