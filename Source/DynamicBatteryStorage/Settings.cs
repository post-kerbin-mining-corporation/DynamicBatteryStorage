using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicBatteryStorage
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class DynamicBatteryStorage : MonoBehaviour
    {
        public static DynamicBatteryStorage Instance { get; private set; }

        protected void Awake()
        {
            Instance = this;
        }
        protected void Start()
        {
            Settings.Load();
        }
    }

    public static class Settings
    {
        public static float TimeWarpLimit = 100f;
        public static float BufferScaling = 1.75f;

        public static void Load()
        {
            ConfigNode settingsNode;


            Utils.Log("Settings: Started loading");
            if (GameDatabase.Instance.ExistsConfigNode("DynamicBatteryStorage/DYNAMICBATTERYSTORAGE"))
            {
                Utils.Log("Settings: Located settings file");

                settingsNode = GameDatabase.Instance.GetConfigNode("DynamicBatteryStorage/DYNAMICBATTERYSTORAGE");

                settingsNode.TryGetValue("MinimumWarpFactor", ref TimeWarpLimit);
                settingsNode.TryGetValue("BufferScaling ", ref BufferScaling);
            }
            else
            {
                Utils.Log("Settings: Couldn't find settings file, using defaults");
            }
            Utils.Log("Settings: Finished loading");
        }
    }
}
