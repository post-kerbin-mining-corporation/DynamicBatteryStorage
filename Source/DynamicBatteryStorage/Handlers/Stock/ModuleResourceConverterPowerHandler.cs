

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

      return UpdatePowerRate();
    }

    // Some converter implementations, including MKS USI_Converter, apply or
    // replace their recipe after the handler is initialized. Re-read the
    // current recipe whenever the monitor calculates a value so editor
    // planning follows the selected bay loadout.
    private bool UpdatePowerRate()
    {
      if (converter == null || converter.inputList == null || converter.outputList == null)
        return false;

      bool toMonitor = false;
      producer = false;
      consumer = false;
      converterEcRate = 0d;

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
      // MKS applies USI_Converter recipes after handler discovery. Keep the
      // handler even when the initial recipe is empty so later calculations
      // can see the selected bay loadout.
      return toMonitor || converter.GetType().Name == "USI_Converter";
    }

    protected override double GetValueEditor()
    {
      if (!UpdatePowerRate())
        return 0d;

      if (producer)
        return converterEcRate;

      if (consumer)
        return -converterEcRate;

      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (!UpdatePowerRate() || !converter.IsActivated)
        return 0d;

      if (producer)
        return converterEcRate * converter.lastTimeFactor;

      if (consumer)
        return converterEcRate * converter.lastTimeFactor * -1.0d;

      return 0d;
    }

    public override string PartTitle()
    {
      return string.Format("{0} ({1})", base.PartTitle(), converter.ConverterName);
    }
  }

}
