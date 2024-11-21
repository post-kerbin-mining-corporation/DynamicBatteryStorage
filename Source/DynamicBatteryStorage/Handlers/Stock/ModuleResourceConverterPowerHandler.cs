

namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleResourceConverter
  /// </summary>
  public class ModuleResourceConverterPowerHandler : ModuleDataHandler
  {
    private ModuleResourceConverter converter;
    private double converterEcRate = 0d;

    public ModuleResourceConverterPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      converter = (ModuleResourceConverter)pm;

      if (converter == null)
        return false;

      bool toMonitor = false;
      for (int i = 0; i < converter.inputList.Count; i++)
      {
        if (converter.inputList[i].ResourceName == Settings.ELECTRICITY_RESOURCE_NAME)
        {
          converterEcRate = converter.inputList[i].Ratio;
          producer = false;
          consumer = true;
          toMonitor = true;
        }
      }
      for (int i = 0; i < converter.outputList.Count; i++)
      {
        if (converter.outputList[i].ResourceName == Settings.ELECTRICITY_RESOURCE_NAME)
        {
          converterEcRate = converter.outputList[i].Ratio;
          producer = true;
          consumer = false;
          toMonitor = true;
        }
      }
      return toMonitor;
    }

    protected override double GetValueEditor()
    {
      if (converter != null)
      {
        if (producer)
          return converterEcRate;
        else
          return -converterEcRate;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (converter != null)
      {
        if (converter.IsActivated)
          if (producer)
            return converterEcRate * converter.lastTimeFactor;
          else
            return converterEcRate * converter.lastTimeFactor * -1.0d;
      }
      return 0d;
    }

    public override string PartTitle()
    {
      return string.Format("{0} ({1})", base.PartTitle(), converter.ConverterName);
    }
  }

}
