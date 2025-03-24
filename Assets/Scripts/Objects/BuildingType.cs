using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Objects
{
    public class BuildingType
    {
        #region GameData
        public string Name { get; set; }

        public string Description { get; set; }

        public BuildingTypeEnum Type { get; set; }

        public int XSize { get; set; }
        
        public int ZSize { get; set; }

        public dynamic PropertyParameters { get; set; }

        public int Cost { get; set; }
        #endregion GameData

        public BuildingProperty PropertyTemplate { get; private set; }

        public Sprite Icon { get; set; }

        public GameObject Prefab;

        public void InitializePropertyTemplate(DataLibrary dataLibrary)
        {
            string par = JsonConvert.SerializeObject((object)PropertyParameters);
            dynamic parameters = new ExpandoObject();
            var parameterDict = JsonConvert.DeserializeObject<IDictionary<string, object>>(par);
            if (!parameterDict.TryGetValue("Type", out var typeValue)) return;

            var typeEnum = (BuildingPropertyTypeEnum)Enum.Parse(typeof(BuildingPropertyTypeEnum), typeValue.ToString());
            var type = (BuildingProperty)Activator.CreateInstance(
                   dataLibrary.BuildingPropertyTypes[typeEnum], new object[] { });

            if (parameterDict.ContainsKey("Parameters") && parameterDict["Parameters"] is JObject parametersObj)
            {
                ExpandoObject parametersExpando = parametersObj.ToObject<ExpandoObject>();
                JsonDataManager.InitializeObject(parametersExpando, type);
            }
            else if (parameterDict["Parameters"] is ExpandoObject expando)
            {
                JsonDataManager.InitializeObject(expando, type);
            }

            PropertyTemplate = type;
        }
    }
}