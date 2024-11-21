
namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleDataTransmitter
  /// </summary>
  public class ModuleDataTransmitterPowerHandler : ModuleDataHandler
  {
    private ModuleDataTransmitter antenna;
    private double cachedCost = 0d;
    public ModuleDataTransmitterPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      antenna = (ModuleDataTransmitter)pm;
      if (antenna == null || antenna.antennaType == AntennaType.INTERNAL)
        return false;

      cachedCost = -antenna.DataResourceCost * (1.0d / antenna.packetInterval);
      return true;
    }

    protected override double GetValueEditor()
    {
      if (antenna != null)
        return cachedCost;
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (antenna != null)
      {
        if (antenna.IsBusy())
          return cachedCost;
      }
      return 0d;
    }
  }


}
