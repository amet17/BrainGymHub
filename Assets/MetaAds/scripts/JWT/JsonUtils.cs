using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class JsonUtils
{
    public static string ToJson(object obj)
    {
        if (obj == null)
        {
            return "null";
        }

        Type type = obj.GetType();
        if (type.IsPrimitive || obj is string)
        {
            return ToJsonPrimitive(obj);
        }

        if (obj is IDictionary)
        {
            return ToJsonDictionary(obj as IDictionary);
        }

        if (obj is IEnumerable)
        {
            return ToJsonArray(obj as IEnumerable);
        }

        return ToJsonObject(obj);
    }

    private static string ToJsonPrimitive(object obj)
    {
        if (obj is string)
        {
            return "\"" + EscapeJsonString(obj.ToString()) + "\"";
        }

        return obj.ToString().ToLowerInvariant();
    }

    private static string ToJsonDictionary(IDictionary dict)
    {
        List<string> keyValuePairs = new List<string>();
        foreach (object key in dict.Keys)
        {
            object value = dict[key];
            keyValuePairs.Add("\"" + key + "\":" + ToJson(value));
        }

        return "{" + string.Join(",", keyValuePairs.ToArray()) + "}";
    }

    private static string ToJsonArray(IEnumerable list)
    {
        List<string> values = new List<string>();
        foreach (object obj in list)
        {
            values.Add(ToJson(obj));
        }

        return "[" + string.Join(",", values.ToArray()) + "]";
    }

/*    private static string ToJsonObject(object obj)
    {
        List<string> keyValuePairs = new List<string>();
        PropertyInfo[] properties = obj.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
        {
            object value = property.GetValue(obj, null);
            keyValuePairs.Add("\"" + property.Name + "\":" + ToJson(value));
        }

        return "{" + string.Join(",", keyValuePairs.ToArray()) + "}";
    }*/
    private static string ToJsonObject(object obj)
    {
        List<string> keyValuePairs = new List<string>();
        PropertyInfo[] properties = obj.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
        {
            object value = property.GetValue(obj, null);
            keyValuePairs.Add("\"" + property.Name + "\":" + ToJson(value));
        }

        return "{" + string.Join(",", keyValuePairs.ToArray()) + "}";
    }

    private static string EscapeJsonString(string str)
    {
        string escaped = str.Replace("\\", "\\\\")
                           .Replace("\"", "\\\"")
                           .Replace("\r", "\\r")
                           .Replace("\n", "\\n")
                           .Replace("\t", "\\t");

        return escaped;
        /*        return str;*/
    }
}
