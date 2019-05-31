using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

  // Fission Reactor
  public class FissionReactorHeatHandler: ModuleDataHandler
  {
    public override double GetValue()
    {
      double results = 0d;
      if (HighLogic.LoadedSceneIsFlight)
      {
        double.TryParse(pm.Fields.GetValue("AvailablePower").ToString(), out results);
        
      }
      else
      {
        float throttle = 100f;
        float.TryParse(pm.Fields.GetValue("CurrentPowerPercent").ToString(), out throttle);
        double.TryParse(pm.Fields.GetValue("HeatGeneration").ToString(), out results);
        results = results * throttle/100f / 50f;
      }
      return results;
    }
  }

  // Flow radiator
  public class FissionFlowRadiatorHeatHandler: ModuleDataHandler
  {
    public override double GetValue()
    {
      double results = 0d;
      if (HighLogic.LoadedSceneIsFlight)
      {
        double.TryParse(pm.Fields.GetValue("currentCooling").ToString(), out results);

      }
      else
      {
        // This currently does not respect the thrust tweakable
        // TODO: Make that happen
        double.TryParse(pm.Fields.GetValue("exhaustCooling").ToString(), out results);
        results /= 50.0d;
      }
      return results;
    }

    public override bool IsProducer()
    {
      return false;
    }
  }
}
