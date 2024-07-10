using System;
using System.Collections.Generic;
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
      enabled = Settings.Enabled;
    }
  }


  public enum ResourcesSupported
  {
    Heat,
    Power
  }

  /// <summary>
  /// Static class to hold settings and configuration
  /// </summary>
  public static class Settings
  {

    public static bool Enabled = true;
    public static float TimeWarpLimit = 100f;
    public static float BufferScaling = 1.75f;
    public static bool DebugMode = true;
    public static bool DebugSettings = true;
    public static bool DebugUIMode = true;
    public static int UIUpdateInterval = 3;
    public static bool DebugLoading = true;

    public static Dictionary<string, UIHandlerCategory> HandlerCategoryData;
    public static List<HandlerModuleData> HandlerPartModuleData;

    /// <summary>
    /// Load data from configuration
    /// </summary>
    public static void Load()
    {
      ConfigNode settingsNode;

      Enabled = CheckForConflictingMods();

      Utils.Log("[Settings]: Started loading");
      if (GameDatabase.Instance.ExistsConfigNode("DynamicBatteryStorage/DYNAMICBATTERYSTORAGE"))
      {
        Utils.Log("[Settings]: Located settings file");

        settingsNode = GameDatabase.Instance.GetConfigNode("DynamicBatteryStorage/DYNAMICBATTERYSTORAGE");

        settingsNode.TryGetValue("MinimumWarpFactor", ref TimeWarpLimit);
        settingsNode.TryGetValue("DebugMode", ref DebugMode);
        settingsNode.TryGetValue("DebugSettings", ref DebugMode);
        settingsNode.TryGetValue("DebugUIMode", ref DebugUIMode);
        settingsNode.TryGetValue("DebugLoading", ref DebugLoading);

        settingsNode.TryGetValue("BufferScaling ", ref BufferScaling);
        settingsNode.TryGetValue("UIUpdateInterval ", ref UIUpdateInterval);
        settingsNode.TryGetValue("Enabled", ref Enabled);

        if (Settings.DebugLoading)
        {
          Utils.Log("[Settings]: Loading handler categories");
        }
        HandlerCategoryData = new Dictionary<string, UIHandlerCategory>();
        ConfigNode[] categoryNodes = settingsNode.GetNodes("HANDLERCATEGORY");

        foreach (ConfigNode node in categoryNodes)
        {
          UIHandlerCategory newCat = new UIHandlerCategory(node);
          HandlerCategoryData.Add(newCat.name, newCat);
        }
        if (Settings.DebugLoading)
        {
          Utils.Log("[Settings]: Loading handler modules");
        }
        HandlerPartModuleData = new List<HandlerModuleData>();
        ConfigNode[] partModuleNodes = settingsNode.GetNodes("PARTMODULEHANDLER");
        foreach (ConfigNode node in partModuleNodes)
        {
          HandlerModuleData newDat = new HandlerModuleData(node);
          HandlerPartModuleData.Add(newDat);
        }

      }
      else
      {
        Utils.Log("[Settings]: Couldn't find settings file, using defaults");
      }
      Utils.Log("[Settings]: Finished loading");
    }

    /// <summary>
    /// Find any conflicting mods, return false if there are any.
    /// The result of this will be overridden by the Enable setting in the plugin configuration.
    /// </summary>
    private static bool CheckForConflictingMods()
    {
      foreach (var a in AssemblyLoader.loadedAssemblies)
      {
        // search for conflicting mods
        if (a.name.StartsWith("Kerbalism", StringComparison.Ordinal))
        {
          Utils.Log("[Settings]: Kerbalism detected. DBS will disable itself.");
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Returns a list of handler categories currently in the mod
    /// </summary>
    public static List<String> HandlerCategories
    {
      get { return new List<string>(Settings.HandlerCategoryData.Keys); }
    }

    /// <summary>
    /// Checks to see if a part module is supported
    /// </summary>
    /// <param name="moduleName">The partModule name to check for.</param>
    /// <param name="resourceType">The type of ResourcesSupported to support</param>
    public static bool IsSupportedPartModule(string moduleName, ResourcesSupported resourceType)
    {
      if (Settings.SupportedModules(resourceType).Contains(moduleName))
        return true;
      return false;
    }

    /// <summary>
    /// Gets the handler module data for a part module name
    /// </summary>
    /// <param name="moduleName">The partModule name to check for.</param>
    /// <param name="resourceType">The type of ResourcesSupported to support</param>
    public static HandlerModuleData GetPartModuleData(string moduleName, ResourcesSupported resourceType)
    {
      for (int i = 0; i < HandlerPartModuleData.Count; i++)
      {
        if (HandlerPartModuleData[i].resourceType == resourceType && HandlerPartModuleData[i].handledModule == moduleName)
          return HandlerPartModuleData[i];
      }
      return null;
    }

    /// <summary>
    /// Gets a list of supported part modules
    /// </summary>
    /// <param name="resourceType">The type of ResourcesSupported to support</param>
    public static List<String> SupportedModules(ResourcesSupported resourceType)
    {
      List<string> supportedModules = new List<string>();
      for (int i = 0; i < HandlerPartModuleData.Count; i++)
      {
        if (HandlerPartModuleData[i].resourceType == resourceType)
          supportedModules.Add(HandlerPartModuleData[i].handledModule);
      }
      return supportedModules;
    }
  }
}
