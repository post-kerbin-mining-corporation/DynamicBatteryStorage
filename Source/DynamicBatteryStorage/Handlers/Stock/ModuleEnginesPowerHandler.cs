
namespace DynamicBatteryStorage
{
  public class ModuleEnginesPowerHandler : ModuleDataHandler
  {
    private ModuleEngines engine;
    private double engineBaseECRate = 0d;
    public ModuleEnginesPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      engine = (ModuleEngines)pm;

      if (engine == null)
        return false;

      bool toMonitor = false;
      double ecRatio = 1.0;
      double massFlowSum = 0d;
      double ratioSum = 0d;
      double totalRate = 0d;
      double massFlowTotal = engine.maxThrust / (9.81 * engine.atmosphereCurve.Evaluate(0f));
      // Computes the base electrical rate in seconds from the propellant ratios
      for (int i = 0; i < engine.propellants.Count; i++)
      {
        if (engine.propellants[i].id == PartResourceLibrary.ElectricityHashcode)
        {
          toMonitor = true;
          ecRatio = engine.propellants[i].ratio;
        }
        else
        {
          ratioSum += engine.propellants[i].ratio;
          massFlowSum += engine.propellants[i].ratio * PartResourceLibrary.Instance.GetDefinition(engine.propellants[i].name).density;
        }
      }
      double mixtureRatio = massFlowSum / ratioSum;

      for (int i = 0; i < engine.propellants.Count; i++)
      {
        if (engine.propellants[i].id != PartResourceLibrary.ElectricityHashcode)
          totalRate += (massFlowTotal / mixtureRatio) * engine.propellants[i].ratio / ratioSum;
      }
      engineBaseECRate = ecRatio / ratioSum * totalRate;
      return toMonitor;
    }

    protected override double GetValueEditor()
    {
      if (engine != null)
      {
        float throttle = engine.thrustPercentage / 100f;
        return -engineBaseECRate * throttle;
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (engine != null)
      {
        if (!engine.engineShutdown)
        {
          return -engineBaseECRate * engine.currentThrottle;
        }
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return string.Format("{0} ({1})", base.PartTitle(), engine.engineID);
    }
  }

  public class ModuleEnginesFXPowerHandler : ModuleEnginesPowerHandler
  {
    public ModuleEnginesFXPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
  }
}
