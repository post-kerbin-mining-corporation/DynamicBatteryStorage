using System;
namespace DynamicBatteryStorage
{
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

    private const string TYPE_PARAMETER_NAME = "type";
    private const string HANDLER_MODULE_PARAMETER_NAME = "handlerModuleName";
    private const string IS_CONSUMER_PARAMETER_NAME = "consumer";
    private const string IS_PRODUCER_PARAMETER_NAME = "producer";
    private const string IS_VISIBLE_PARAMETER_NAME = "visible";
    private const string IS_SOLAR_PARAMETER_NAME = "solarEfficiencyEffects";
    private const string IS_SIMULATED_PARAMETER_NAME = "simulated";
    private const string IS_CONTINUOUS_PARAMETER_NAME = "continuous";

    private const string GENERIC_HANDLER_NAME = "GenericFieldDataHandler";
    private const string HANDLER_CONFIG_NODE_NAME = "HANDLER_CONFIG";

    public HandlerConfiguration config;

    public HandlerModuleData(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      handledModule = node.GetValue("name");
      Utils.TryParseEnum<ResourcesSupported>(node.GetValue(TYPE_PARAMETER_NAME), false, out resourceType);
      node.TryGetValue(HANDLER_MODULE_PARAMETER_NAME, ref handlerModuleName);

      node.TryGetValue(IS_CONSUMER_PARAMETER_NAME, ref consumer);
      node.TryGetValue(IS_PRODUCER_PARAMETER_NAME, ref producer);
      node.TryGetValue(IS_VISIBLE_PARAMETER_NAME, ref visible);
      node.TryGetValue(IS_SOLAR_PARAMETER_NAME, ref solarEfficiencyEffects);
      node.TryGetValue(IS_SIMULATED_PARAMETER_NAME, ref simulated);
      node.TryGetValue(IS_CONTINUOUS_PARAMETER_NAME, ref continuous);

      if (handlerModuleName == GENERIC_HANDLER_NAME)
      {
        config = new HandlerConfiguration(node.GetNode(HANDLER_CONFIG_NODE_NAME));
      }
      Utils.Log(String.Format("[Settings]: Loaded {0}", this.ToString()), Utils.LogType.Settings);
    }

    public override string ToString()
    {
      return String.Format("Type {0} ({1})", handledModule, resourceType);
    }
  }

}
