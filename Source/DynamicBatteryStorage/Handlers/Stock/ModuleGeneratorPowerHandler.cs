
namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleGenerator
  /// </summary>
  public class ModuleGeneratorPowerHandler : ModuleDataHandler
  {
    private ModuleGenerator gen;
    private ModuleResource resource;

    public ModuleGeneratorPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      gen = (ModuleGenerator)pm;
      if (gen == null)
        return false;

      bool toMonitor = false;
      for (int i = 0; i < gen.resHandler.inputResources.Count; i++)
      {
        if (gen.resHandler.inputResources[i].id == PartResourceLibrary.ElectricityHashcode)
        {
          producer = false;
          consumer = true;
          resource = gen.resHandler.inputResources[i];
          toMonitor = true;
        }
      }

      for (int i = 0; i < gen.resHandler.outputResources.Count; i++)
      {
        if (gen.resHandler.outputResources[i].id == PartResourceLibrary.ElectricityHashcode)
        {
          producer = true;
          consumer = false;
          resource = gen.resHandler.inputResources[i];
          toMonitor = true;
        }
      }
      return toMonitor;

    }
    protected override double GetValueEditor()
    {
      if (gen != null && resource != null)
      {
        if (Producer)
          return resource.rate;
        else
          return -resource.rate;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (gen != null && resource != null)
      {
        if (gen.generatorIsActive)
        {
          if (producer)
            return gen.efficiency * resource.rate;
          else
            return gen.efficiency * resource.rate * -1.0d;
        }
      }
      return 0d;
    }
  }

}
