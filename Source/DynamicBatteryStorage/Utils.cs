using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Debug.Warn(String.Format("[{0}]: {1}", logTag, toLog));
        }
        public static void Error(string toLog)
        {
            Debug.Error(String.Format("[{0}]: {1}", logTag, toLog));
        }
    }
}
