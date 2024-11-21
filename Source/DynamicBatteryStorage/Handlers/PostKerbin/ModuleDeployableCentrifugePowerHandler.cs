namespace DynamicBatteryStorage
{
  public class ModuleDeployableCentrifugePowerHandler : ModuleDataHandler
  {
    public ModuleDeployableCentrifugePowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      Utils.TryGetField(pm, "SpinResourceRate", out double results);
      return results != 0d;
    }

    protected override double GetValueEditor()
    {
      Utils.TryGetField(pm, "SpinResourceRate", out double results);
      return -results;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      Utils.TryGetField(pm, "Rotating", out bool on);      
      if (on)
      {
        Utils.TryGetField(pm, "SpinResourceRate", out results);
      }
      return -results;
    }
  }
}
