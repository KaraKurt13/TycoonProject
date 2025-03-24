using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SystemExtensions
{
    public static T Clone<T>(this T source)
    {
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        var serialized = JsonConvert.SerializeObject(source, settings);
        return JsonConvert.DeserializeObject<T>(serialized, settings);
    }
}
