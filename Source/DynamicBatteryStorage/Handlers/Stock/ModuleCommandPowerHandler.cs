
namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleCommand
  /// </summary>
  public class ModuleCommandPowerHandler : ModuleDataHandler
  {
    private ModuleCommand pod;
    private ModuleResource resource;
    public ModuleCommandPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      pod = (ModuleCommand)pm;

      if (pod == null)
        return false;

      // Test to see if the ModuleCommand actually uses power
      for (int i = 0; i < pod.resHandler.inputResources.Count; i++)
      {
        if (pod.resHandler.inputResources[i].id == PartResourceLibrary.ElectricityHashcode)
        {
          if (pod.resHandler.inputResources[i].rate > 0.0d)
          {
            resource = pod.resHandler.inputResources[i];
            return true;
          }
        }
      }
      return false;
    }

    protected override double GetValueEditor()
    {
      if (resource != null)
      {
        return -resource.rate;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (pod != null && !pod.IsHibernating && resource != null)
      {
        return -resource.rate;
      }
      return 0d;
    }
  }
}
