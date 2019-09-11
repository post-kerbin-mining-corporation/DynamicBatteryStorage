using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;

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

    public static float TimeWarpLimit = 100f;
    public static float BufferScaling = 1.75f;
    public static bool DebugMode = true;
    public static bool DebugSettings = true;
    public static bool DebugUIMode = true;
    public static int UIUpdateInterval = 3;


    public static Dictionary<string, HandlerCategory> HandlerCategoryData;
    public static List<HandlerModuleData> HandlerPartModuleData;

    /// <summary>
    /// Load data from configuration
    /// </summary>
    public static void Load()
    {
      ConfigNode settingsNode;

      Utils.Log("[Settings]: Started loading");
      if (GameDatabase.Instance.ExistsConfigNode("DynamicBatteryStorage/DYNAMICBATTERYSTORAGE"))
      {
        Utils.Log("[Settings]: Located settings file");

        settingsNode = GameDatabase.Instance.GetConfigNode("DynamicBatteryStorage/DYNAMICBATTERYSTORAGE");

        settingsNode.TryGetValue("MinimumWarpFactor", ref TimeWarpLimit);
        settingsNode.TryGetValue("DebugMode", ref DebugMode);
        settingsNode.TryGetValue("DebugSettings", ref DebugMode);
        settingsNode.TryGetValue("DebugUIMode", ref DebugUIMode);
        settingsNode.TryGetValue("BufferScaling ", ref BufferScaling);
        settingsNode.TryGetValue("UIUpdateInterval ", ref UIUpdateInterval);

        Utils.Log("[Settings]: Loading handler categories");
        HandlerCategoryData = new Dictionary<string, HandlerCategory>();
        ConfigNode[] categoryNodes = settingsNode.GetNodes("HANDLERCATEGORY");

        foreach (ConfigNode node in categoryNodes)
        {
          HandlerCategory newCat = new HandlerCategory(node);
          HandlerCategoryData.Add(newCat.name, newCat);
        }

        Utils.Log("[Settings]: Loading handler modules");
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
    /// Returns a list of handler categories currently in the mod
    /// </summary>
    public static List<String> HandlerCategories {
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
    public static List<String> SupportedModules(ResourcesSupported resourceType) {
      List<string> supportedModules = new List<string>();
      for (int i = 0; i < HandlerPartModuleData.Count; i++)
      {
        if (HandlerPartModuleData[i].resourceType == resourceType)
          supportedModules.Add(HandlerPartModuleData[i].handledModule);
      }
      return supportedModules;
    }
  }

  /// <summary>
  /// Defines a part module category for the UI
  /// </summary>
  public class HandlerCategory
  {
    public string name ;
    public List<string> handledModules;
    public string title;

    public HandlerCategory(ConfigNode node)
    {
      Load(node);
    }
    public void Load(ConfigNode node)
    {
      name = node.GetValue("name");
      title = Localizer.Format(node.GetValue("title"));

      handledModules = node.GetValuesList("module");
      Utils.Log(String.Format("[Settings]: Loaded {0}", this.ToString()));
    }

    public string ToString()
    {
      return String.Format("{0}", name);
    }
  }

  /// <summary>
  /// Defines a set of data for a part module handler to be set up with
  /// </summary>
  public class HandlerModuleData
  {
    public string handledModule;
    public string handlerModuleName;
    public ResourcesSupported resourceType;

    public bool consumer = false;
    public bool producer = false;
    public bool solarEfficiencyEffects = false;
    public bool visible = true;
    public bool continuous = true;
    public bool simulated = true;

    public HandlerConfiguration config;

    public HandlerModuleData(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      handledModule = node.GetValue("name");
      Utils.TryParseEnum<ResourcesSupported>(node.GetValue("type"), false, out resourceType);
      node.TryGetValue("handlerModuleName", ref handlerModuleName);

      node.TryGetValue("consumer", ref consumer);
      node.TryGetValue("producer", ref producer);
      node.TryGetValue("visible", ref visible);
      node.TryGetValue("solarEfficiencyEffects", ref solarEfficiencyEffects);
      node.TryGetValue("simulated", ref simulated);
      node.TryGetValue("continuous", ref continuous);

      if (handlerModuleName == "GenericFieldDataHandler")
      {
        config = new HandlerConfiguration(node.GetNode("HANDLER_CONFIG"));
      }
      Utils.Log(String.Format("[Settings]: Loaded {0}", this.ToString()));
    }

    public string ToString()
    {
      return String.Format("Type {0} ({1})", handledModule, resourceType);
    }
  }


  /// <summary>
  /// Defines data for a sepecific type of handler
  /// </summary>
  public class HandlerConfiguration
  {
    public string editorFieldName = "";
    public string flightFieldName = "";

    public double editorValueScalar = 1.0d;
    public double flightValueScalar = 1.0d;

    public HandlerConfiguration(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue("editorFieldName", ref editorFieldName);
      node.TryGetValue("flightFieldName", ref flightFieldName);
      node.TryGetValue("flightValueScalar", ref flightValueScalar);
      node.TryGetValue("editorValueScalar", ref editorValueScalar);

      if (editorFieldName == "" && flightFieldName == "")
      {
        Utils.Log("At least one of editorFieldName or flightFieldName must be specified in HANDLER_CONFIG for GenericFieldDataHandler");
      } else if (editorFieldName == "")
      {
        editorFieldName = flightFieldName;
      } else if (flightFieldName == "")
      {
        flightFieldName = editorFieldName;
      }
    }

  }
}
