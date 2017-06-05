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
    }
}
