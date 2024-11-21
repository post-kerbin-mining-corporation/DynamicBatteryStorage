namespace DynamicBatteryStorage
{
  public class ModuleChargeableEnginePowerHandler : ModuleDataHandler
  {

    private bool hasOfflineGenerator = false;
    private double offlineBaseRate = 0d;
    public ModuleChargeableEnginePowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      Utils.TryGetField(pm, "PowerGeneratedOffline", out hasOfflineGenerator);
      if (hasOfflineGenerator)
      {
        Utils.TryGetField(pm, "PowerGenerationTotal", out offlineBaseRate);
        producer = true;
      }
      base.Initialize(pm);
      return true;
    }

    protected override double GetValueEditor()
    {
      double results = 0d;
      Utils.TryGetField(pm, "Charging", out bool charging);

      if (charging)
      {
        Utils.TryGetField(pm, "ChargeRate", out results);
        results *= -1d;
      }
      if (!charging && hasOfflineGenerator)
      {
        Utils.TryGetField(pm, "GeneratorRate", out float genRate);
        results = offlineBaseRate * genRate / 100f;
      }
      return results;
    }
    protected override double GetValueFlight()
    {
      double results;
      Utils.TryGetField(pm, "Charging", out bool charging);

      if (charging)
      {
        Utils.TryGetField(pm, "ChargeRate", out results);
        results *= -1d;
      }
      else
      {
        Utils.TryGetField(pm, "PowerGenerationTotal", out results);
        return results;
      }
      return results;
    }
  }
}
