namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleActiveRadiator
  /// </summary>
  public class ModuleActiveRadiatorPowerHandler : ModuleDataHandler
  {
    private ModuleActiveRadiator radiator;
    private ModuleResource resource;
    public ModuleActiveRadiatorPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      radiator = (ModuleActiveRadiator)pm;
      if (radiator == null)
        return false;

      bool toMonitor = false;
      for (int i = 0; i < radiator.resHandler.inputResources.Count; i++)
      {
        if (radiator.resHandler.inputResources[i].id == PartResourceLibrary.ElectricityHashcode)
        {
          resource = radiator.resHandler.inputResources[i];
          toMonitor = true;
        }
      }
      return toMonitor;
    }

    protected override double GetValueEditor()
    {
      if (radiator != null && resource != null)
      {
        return -resource.rate;
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (radiator != null && resource != null)
      {
        if (radiator.IsCooling)
        {
          return -resource.rate;
        }
      }
      return 0d;
    }
  }

}
