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
    public FissionReactorHeatHandler(HandlerModuleData moduleData):base(moduleData)
    {}

    protected override double GetValueEditor()
    {
      double results = 0d;
      float throttle = 100f;
      // Ensure we object the reactor slider
      float.TryParse(pm.Fields.GetValue("CurrentPowerPercent").ToString(), out throttle);
      double.TryParse(pm.Fields.GetValue("HeatGeneration").ToString(), out results);
      results = results * throttle/100f / 50f;

      return results;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("AvailablePower").ToString(), out results);
      return results;
    }

  }

  // Flow radiator
  public class FissionFlowRadiatorHeatHandler: ModuleDataHandler
  {
    ModuleEnginesFX engine;

    public FissionFlowRadiatorHeatHandler(HandlerModuleData moduleData):base(moduleData)
    {}

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      engine = (ModuleEnginesFX)pm;
      return true;
    }

    protected override double GetValueEditor()
    {
      double results = 0d;
      if (engine != null)
      {
        double.TryParse(pm.Fields.GetValue("exhaustCooling").ToString(), out results);
        results = results/50.0d * engine.thrustPercentage / 100f;;
      }
      return results;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("currentCooling").ToString(), out results);
      return results;
    }
  }
}
