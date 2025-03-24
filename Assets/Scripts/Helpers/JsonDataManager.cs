using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class JsonDataManager
{
    private const string _dataPath = "Assets/Data/GameData.json";

    private static Dictionary<string, object> _data;

    private static Dictionary<string, Type> _typeCache;

    public static void Initialize()
    {
        var jsonContent = File.ReadAllText(_dataPath);
        _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
        _typeCache = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsNestedPrivate && !t.Name.StartsWith("<") &&
                t.Namespace != null && t.Namespace.StartsWith("Assets.Scripts.Objects"))
                .ToDictionary(t => t.Name, t => t);
    }

    public static Dictionary<TEnum, TType> DeserializeObjects<TEnum, TType>(string name)
        where TEnum : Enum
        where TType : class
    {
        var dataChapter = _data[name];
        var dictionary = new Dictionary<TEnum, TType>();
        var currentType = typeof(TType);

        if (dataChapter is JObject jObject)
        {
            dictionary = jObject.ToObject<Dictionary<TEnum, TType>>();
        }
        else if (dataChapter is JArray jArray)
        {
            foreach (var item in jArray)
            {
                JObject obj = item as JObject;
                if (obj == null)
                {
                    throw new ArgumentException($"Can't deserialize object in {name} dictionary!");
                }

                var property = currentType.GetProperty("Type");
                var objToCreate = Activator.CreateInstance(currentType);

                foreach (var prop in currentType.GetProperties())
                {
                    if (item[prop.Name] != null)
                    {
                        if (prop.PropertyType.IsAbstract || prop.PropertyType.IsInterface)
                        {
                            var token = item[prop.Name];
                            var derivedTypeName = token["Type"].ToString();
                            var derivedType = _typeCache[derivedTypeName];

                            if (derivedType == null)
                            {
                                throw new ArgumentException($"Type {derivedTypeName} not found in assembly.");
                            }

                            var parameters = token["Parameters"];
                            var v = parameters.ToObject(derivedType);
                            prop.SetValue(objToCreate, v);
                        }
                        else
                        {
                            var v = item[prop.Name].ToObject(prop.PropertyType);
                            prop.SetValue(objToCreate, v);
                        }
                    }
                }

                if (property == null)
                {
                    throw new ArgumentException($"Property TYPE not found in type {currentType.Name}");
                }

                var value = (TEnum)Enum.Parse(typeof(TEnum), property.GetValue(objToCreate).ToString());
                dictionary.Add(value, objToCreate as TType);
            }
        }

        return dictionary;
    }

    public static void InitializeObject(ExpandoObject data, object obj)
    {
        if (data == null || obj == null) return;

        var objType = obj.GetType();
        var dict = data as IDictionary<string, object>;

        foreach (var kvp in dict)
        {
            var propertyName = kvp.Key;
            var value = kvp.Value;

            var property = objType.GetProperty(propertyName);

            if (property == null)
            {
                Debug.LogWarning($"Property '{propertyName}' not found on type '{objType}'");
                continue;
            }

            var propertyType = property.PropertyType;

            try
            {
                if (propertyType == typeof(int) && value is long)
                {
                    value = Convert.ToInt32(value);
                }
                else if (propertyType == typeof(float) && value is double)
                {
                    value = Convert.ToSingle(value);
                }
                else if (propertyType.IsEnum && value is string)
                {
                    value = Enum.Parse(propertyType, (string)value);
                }
                else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var itemType = propertyType.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(propertyType);

                    foreach (var item in (IEnumerable<object>)value)
                    {
                        if (item is IDictionary<string, object> itemDict && itemDict.ContainsKey("Type") && itemDict.ContainsKey("Parameters"))
                        {
                            var derivedTypeName = itemDict["Type"].ToString();
                            var derivedType = _typeCache[derivedTypeName] ?? throw new ArgumentException($"Type {derivedTypeName} not found.");

                            var newInstance = Activator.CreateInstance(derivedType);
                            InitializeObject(itemDict["Parameters"] as ExpandoObject, newInstance);
                            list.Add(newInstance);
                        }
                        else
                        {
                            Debug.Log(itemType);
                            var serializedItem = JsonConvert.SerializeObject(item);
                            var deserializedItem = JsonConvert.DeserializeObject(serializedItem, itemType);
                            list.Add(deserializedItem);
                        }
                    }

                    value = list;
                }
                else if (propertyType.IsAbstract || propertyType.IsInterface)
                {
                    if (value is IDictionary<string, object> abstractDict && abstractDict.ContainsKey("Type") && abstractDict.ContainsKey("Parameters"))
                    {
                        var derivedTypeName = abstractDict["Type"].ToString();
                        var derivedType = _typeCache[derivedTypeName] ?? throw new ArgumentException($"Type {derivedTypeName} not found.");

                        var newInstance = Activator.CreateInstance(derivedType);
                        InitializeObject(abstractDict["Parameters"] as ExpandoObject, newInstance);
                        value = newInstance;
                    }
                }
                else
                {
                    if (!propertyType.IsInstanceOfType(value))
                    {
                        value = Convert.ChangeType(value, propertyType);
                    }
                }

                property.SetValue(obj, value);
            }
            catch (Exception ex)
            {
                foreach (var t in _typeCache)
                {
                    Debug.Log(t.Key);
                }
                Debug.LogError($"Failed to set property '{propertyName}' on type '{objType}': {ex.Message}");
            }
        }
    }
}
