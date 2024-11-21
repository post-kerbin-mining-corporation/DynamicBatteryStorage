
namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleAlternator
  /// </summary>
  public class ModuleAlternatorPowerHandler : ModuleDataHandler
  {
    private ModuleAlternator alternator;
    private ModuleEngines[] engines;
    private ModuleResource resource;

    public ModuleAlternatorPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      alternator = (ModuleAlternator)pm;

      if (alternator == null)
        return false;

      engines = pm.part.GetComponents<ModuleEngines>();

      // Test to see if the alternator actually produces power
      for (int i = 0; i < alternator.resHandler.outputResources.Count; i++)
      {
        if (alternator.resHandler.outputResources[i].id == PartResourceLibrary.ElectricityHashcode)
        {
          if (alternator.resHandler.outputResources[i].rate > 0.0d)
          {
            resource = alternator.resHandler.outputResources[i];
            return true;
          }
        }
      }
      return false;
    }

    protected override double GetValueEditor()
    {
      if (alternator != null)
      {
        return resource.rate;
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (alternator != null && engines != null)
      {
        if (alternator.preferMultiMode)
        {
          foreach (ModuleEngines e in engines)
          {
            if (e.isOperational)
            {
              return alternator.outputRate;
            }
          }
        }
        else
        {
          foreach (ModuleEngines e in engines)
          {
            if (e.isOperational)
            {
              return alternator.outputRate;
            }
            else
              return 0f;
          }
        }
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return string.Format("{0} (Alternator)", base.PartTitle());
    }
  }

}
