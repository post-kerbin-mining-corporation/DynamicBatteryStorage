using System;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicBatteryStorage
{
  public static class Utils
  {
    public static string logTag = "Dynamic Battery Storage";

    public static void Log(string toLog)
    {
      Debug.Log(String.Format("[{0}]: {1}", logTag, toLog));
    }
    public static void Warn(string toLog)
    {
      Debug.LogWarning(String.Format("[{0}]: {1}", logTag, toLog));
    }
    public static void Error(string toLog)
    {
      Debug.LogError(String.Format("[{0}]: {1}", logTag, toLog));
    }

    public static bool TryParseEnum<T>(string str, bool caseSensitive, out T value) where T : struct
    {
      // Can't make this a type constraint...
      if (!typeof(T).IsEnum)
      {
        throw new ArgumentException("Type parameter must be an enum");
      }
      var names = Enum.GetNames(typeof(T));
      value = (Enum.GetValues(typeof(T)) as T[])[0];  // For want of a better default
      foreach (var name in names)
      {
        if (String.Equals(name, str, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
        {
          value = (T)Enum.Parse(typeof(T), name);
          return true;
        }
      }
      return false;
    }
    /// <summary>
    /// Get a reference in a child of a type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static T FindChildOfType<T>(string name, Transform parent)
    {
      T result = default(T);
      try
      {
        result = parent.FindDeepChild(name).GetComponent<T>();
      }
      catch (NullReferenceException e)
      {
        Debug.LogError($"Couldn't find {name} in children of {parent.name}");
      }
      return result;
    }
  }
}

public static class TransformDeepChildExtension
{
  //Breadth-first search
  public static Transform FindDeepChild(this Transform aParent, string aName)
  {
    Queue<Transform> queue = new Queue<Transform>();
    queue.Enqueue(aParent);
    while (queue.Count > 0)
    {
      var c = queue.Dequeue();
      if (c.name == aName)
        return c;
      foreach (Transform t in c)
        queue.Enqueue(t);
    }
    return null;
  }
}


