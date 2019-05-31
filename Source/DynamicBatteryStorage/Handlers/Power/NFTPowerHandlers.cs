using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

    // Curved Solar Panel
    public class ModuleCurvedSolarPanelPowerHandler: ModuleDataHandler
    {
      public override double GetValue()
      {

        double results = 0d;

        if (HighLogic.LoadedSceneIsEditor)
        {
          double.TryParse(pm.Fields.GetValue("TotalEnergyRate").ToString(), out results);
        }
        else
        {
          double.TryParse(pm.Fields.GetValue("energyFlow").ToString(), out results);
        }
        return results;
      }
      public override double GetValue(float scalar)
      {
        return GetValue() * scalar;
      }
      public override bool AffectedBySunDistance()
      {
        return true;
      }

    }

    // Fission Reactor
  public class FissionGeneratorPowerHandler: ModuleDataHandler
  {
    PartModule core;
    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      for (int i = 0;i<pm.part.Modules.Count;  i++)
      {
        if (pm.part.Modules[i].moduleName == "FissionReactor")
        {
          core = pm.part.Modules[i];
        }
      }
    }

    public override double GetValue()
    {
      double results = 0d;
      if (HighLogic.LoadedSceneIsEditor)
      {
        float throttle = 100f;
        float.TryParse(core.Fields.GetValue("CurrentPowerPercent").ToString(), out throttle);
        double.TryParse(pm.Fields.GetValue("PowerGeneration").ToString(), out results);
        results = throttle / 100f * results;
      }
      else if (HighLogic.LoadedSceneIsFlight)
      {
        double.TryParse(pm.Fields.GetValue("CurrentGeneration").ToString(), out results);
      }
      return results;
    }
  }

  // RTG
  public class ModuleRadioisotopeGeneratorPowerHandler: ModuleDataHandler
  {
    public override double GetValue()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("ActualPower").ToString(), out results);
      return results;
    }
  }

  // CryoTank
  public class ModuleCryoTankPowerHandler: ModuleDataHandler
  {
    public override double GetValue()
    {
      double resAmt = GetMaxFuelAmt();
      double results = 0d;
      if (resAmt > 0d)
      {

        if (HighLogic.LoadedSceneIsEditor)
        {
          double.TryParse(pm.Fields.GetValue("CoolingCost").ToString(), out results);

          return results * (resAmt / 1000d) * -1d;
        }
        double.TryParse(pm.Fields.GetValue("currentCoolingCost").ToString(), out results);
      } else
      {
        visible = false;
      }
      return results * -1.0d;
    }
    protected double GetMaxFuelAmt()
    {
      double max = 0d;
      int id = PartResourceLibrary.Instance.GetDefinition("LqdHydrogen").id;
      PartResource res = pm.part.Resources.Get(id);
      if (res != null)
        max += res.maxAmount;
      return max;
    }
      public override bool IsProducer()
      {
          return false;
      }
    }

    // Antimatter Tank
    public class ModuleAntimatterTankPowerHandler: ModuleDataHandler
    {
      public override double GetValue()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("ContainmentCostCurrent").ToString(), out results);
        return results* -1.0d;
      }
      public override bool IsProducer()
      {
          return false;
      }
    }

    // Chargeable Engine
    public class ModuleChargeableEnginePowerHandler: ModuleDataHandler
    {
      public override double GetValue()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("ChargeRate").ToString(), out results);
        return results* -1.0d;
      }
      public override bool IsProducer()
      {
          return false;
      }
    }

  // Centrifuge
  public class ModuleDeployableCentrifugePowerHandler : ModuleDataHandler
  {
    public override double GetValue()
    {

      double results = 0d;
      bool on = false;
      if (HighLogic.LoadedSceneIsEditor)
      {
        double.TryParse(pm.Fields.GetValue("SpinResourceRate").ToString(), out results);
        if (results == 0d)
        {
          visible = false;
        }
      }
      else
      {
        bool.TryParse(pm.Fields.GetValue("Rotating").ToString(), out on);
        if (on)
          double.TryParse(pm.Fields.GetValue("SpinResourceRate").ToString(), out results);
        
      }
      return -results;
    }
   
    public override bool IsProducer()
    {
      return false;
    }

  }
}
