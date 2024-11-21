namespace DynamicBatteryStorage
{
  public class FissionGeneratorPowerHandler : ModuleDataHandler
  {
    public FissionGeneratorPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    PartModule core;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      for (int i = 0; i < pm.part.Modules.Count; i++)
      {
        if (pm.part.Modules[i].moduleName == "FissionReactor")
        {
          core = pm.part.Modules[i];
        }
      }
      return true;
    }

    protected override double GetValueEditor()
    {
      // Ensure we respect reactor throttle
      Utils.TryGetField(pm, "CurrentPowerPercent", out float throttle);
      Utils.TryGetField(pm, "PowerGeneration", out double results);
      results = throttle / 100f * results;

      return results;
    }
    protected override double GetValueFlight()
    {
      Utils.TryGetField(pm, "CurrentGeneration", out double results);
      return results;
    }
  }
}
