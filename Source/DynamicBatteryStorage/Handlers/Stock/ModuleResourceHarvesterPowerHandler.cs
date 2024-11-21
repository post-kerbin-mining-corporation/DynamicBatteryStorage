
namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleResourceHarvester
  /// </summary>
  public class ModuleResourceHarvesterPowerHandler : ModuleDataHandler
  {

    private ModuleResourceHarvester harvester;
    private double converterEcRate = 0.0d;

    public ModuleResourceHarvesterPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      harvester = (ModuleResourceHarvester)pm;

      if (harvester == null)
        return false;

      bool toMonitor = false;
      for (int i = 0; i < harvester.inputList.Count; i++)
      {
        if (harvester.inputList[i].ResourceName == Settings.ELECTRICITY_RESOURCE_NAME)
        {
          converterEcRate = harvester.inputList[i].Ratio;
          toMonitor = true;
        }
      }
      return toMonitor;
    }

    protected override double GetValueEditor()
    {
      if (harvester != null)
      {
        return -converterEcRate;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (harvester != null)
      {
        if (harvester.IsActivated)
          return converterEcRate * harvester.lastTimeFactor * -1.0d;
      }
      return 0d;
    }

    public override string PartTitle()
    {
      return string.Format("{0} ({1})", base.PartTitle(), harvester.ConverterName);
    }
  }

}
