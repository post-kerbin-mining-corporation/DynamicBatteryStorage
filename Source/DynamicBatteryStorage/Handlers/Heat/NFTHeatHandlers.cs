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
        double.TryParse(pm.Fields.GetValue("AvailablePower").ToString(), out results);
        return results;
      }
    }

    // Flow radiator
    public class FissionFlowRadiatorHeatHandler: ModuleDataHandler
    {
      public override double GetValue()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("currentCooling").ToString(), out results);
        return results;
      }

      public override bool IsProducer()
      {
        return false;
      }
    }
}
