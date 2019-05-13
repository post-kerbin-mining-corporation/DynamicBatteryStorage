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
        double.TryParse(pm.Fields.GetValue("HeatGeneration").ToString(), out results);
        results /= 50.0d;
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
