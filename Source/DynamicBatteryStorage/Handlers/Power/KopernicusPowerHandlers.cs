using System;

namespace DynamicBatteryStorage
{

  // Special power handler for Kopernicus's replaced panels
  public class KopernicusSolarPanelPowerHandler : ModuleDataHandler
  {
   
    public KopernicusSolarPanelPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      /// If kopernicus is using the multistar settings, we need to only show this thing in flight
      if (Settings.KopernicusMultiStar)
      {
        if (HighLogic.LoadedSceneIsEditor)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
      else
      {
        return true;
      }
    }

    protected override double GetValueEditor()
    {
      return 0f;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      if (double.TryParse(pm.Fields.GetValue("currentOutput").ToString(), out results))
      {
        return results;
      }
      return results;

    }
  }

}
