using System;

namespace DynamicBatteryStorage
{
  public class SCANsatPowerHandler : ModuleDataHandler
  {
    public SCANsatPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    protected override double GetValueEditor()
    {
      for (int i = pm.resHandler.inputResources.Count - 1; i >= 0; i--)
      {
        if (pm.resHandler.inputResources[i].name == "ElectricCharge")
        {
          return -(float)pm.resHandler.inputResources[i].rate;


        }
      }

      return 0f;
    }
    protected override double GetValueFlight()
    {
      bool results = false;
      bool.TryParse(pm.Fields.GetValue("scanning").ToString(), out results);

      if (results)
      {
        for (int i = pm.resHandler.inputResources.Count - 1; i >= 0; i--)
        {
          if (pm.resHandler.inputResources[i].name == "ElectricCharge")
          {
            return -(float)pm.resHandler.inputResources[i].rate;
          }
        }
      }
      return 0f;

    }
  }

}
