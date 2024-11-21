namespace DynamicBatteryStorage
{
  public class DischargeCapacitorPowerHandler : ModuleDataHandler
  {
    public DischargeCapacitorPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);

      bool discharging = false;
      Utils.TryGetField(pm, "Enabled", out bool charging);
      if (charging)
        producer = false;
      if (discharging)
        producer = true;

      return true;
    }

    protected override double GetValueEditor()
    {
      double results = 0d;
      Utils.TryGetField(pm, "Enabled", out bool charging);
      Utils.TryGetField(pm, "Discharging", out bool discharging);
      if (charging)
      {
        Utils.TryGetField(pm, "ChargeRate", out results);
        results *= -1d;
      }
      if (discharging)
      {
        Utils.TryGetField(pm, "dischargeActual", out results);
      }

      if (charging)
        producer = false;
      if (discharging)
        producer = true;

      return results;
    }
    protected override double GetValueFlight()
    {

      double results = 0d;
      Utils.TryGetField(pm, "Enabled", out bool charging);
      Utils.TryGetField(pm, "Discharging", out bool discharging);
      if (charging)
      {
        Utils.TryGetField(pm, "ChargeRate", out results);
        results *= -1d;
      }
      if (discharging)
      {
        Utils.TryGetField(pm, "dischargeActual", out results);
      }

      if (charging)
        producer = false;
      if (discharging)
        producer = true;
      return results;
    }
  }
}
