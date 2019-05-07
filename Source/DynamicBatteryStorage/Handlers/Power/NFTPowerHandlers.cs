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
        double.TryParse(pm.Fields.GetValue("energyFlow").ToString(), out results);
        return results;
      }
    }

    // Fission Reactor
    public class FissionGeneratorPowerHandler: ModuleDataHandler
    {
      public override double GetValue()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("CurrentGeneration").ToString(), out results);
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
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("currentCoolingCost").ToString(), out results);
        return results * -1.0d;
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
}
