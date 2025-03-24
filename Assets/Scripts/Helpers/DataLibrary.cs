using Assets.Scripts.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class DataLibrary
{
    public Dictionary<BuildingTypeEnum, BuildingType> BuildingTypes;

    public Dictionary<BuildingPropertyTypeEnum, Type> BuildingPropertyTypes;

    public void Initialize()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(BuildingProperty)) && !t.IsAbstract);
        BuildingPropertyTypes = new();
        foreach (var type in types)
        {
            var typeEnum = Enum.Parse<BuildingPropertyTypeEnum>(type.Name);
            BuildingPropertyTypes.Add(typeEnum, type);
        }
        BuildingTypes = JsonDataManager.DeserializeObjects<BuildingTypeEnum, BuildingType>("BuildingTypes");
        foreach (var type in BuildingTypes.Values)
        {
            type.Prefab = Resources.Load<GameObject>($"Prefabs/Buildings/{type.Type}");
            type.InitializePropertyTemplate(this);

        }
    }
}
