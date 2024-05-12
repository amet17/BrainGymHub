using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JWT;

public class UnityJwtJsonSerializer : IJsonSerializer
{
    T IJsonSerializer.Deserialize<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    string IJsonSerializer.Serialize(object obj)
    {
        if (obj is string)
        {
            return obj.ToString();
        }
       return JsonUtils.ToJson(obj);
       
    }
}
