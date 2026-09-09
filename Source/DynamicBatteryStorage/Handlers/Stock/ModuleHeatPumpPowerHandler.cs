namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleHeatPump derives from ModuleActiveRadiator and exposes its
  /// ElectricCharge draw through the same resource handler.
  /// </summary>
  public class ModuleHeatPumpPowerHandler : ModuleActiveRadiatorPowerHandler
  {
    public ModuleHeatPumpPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
  }
}
