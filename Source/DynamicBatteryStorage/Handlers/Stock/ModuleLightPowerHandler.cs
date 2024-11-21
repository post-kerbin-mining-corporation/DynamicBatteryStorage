

namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleLight
  /// </summary>
  public class ModuleLightPowerHandler : ModuleDataHandler
  {
    private ModuleLight light;

    public ModuleLightPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      light = (ModuleLight)pm;
      if (light == null)
        return false;
      return true;
    }

    protected override double GetValueFlight()
    {
      if (light != null)
        if (light.isOn)
          return -light.resourceAmount;
      return 0d;
    }
    protected override double GetValueEditor()
    {
      if (light != null)
        return -light.resourceAmount;
      return 0d;
    }
  }


}
