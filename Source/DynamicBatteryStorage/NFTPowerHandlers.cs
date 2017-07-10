using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

    // Curved Solar Panel
    public class ModuleCurvedSolarPanelHandler: PowerHandler
    {
        public override double GetPower()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("energyFlow").ToString(), out results);
        return results;
      }
    }

    // Fission Reactor
    public class FissionGeneratorHandler: PowerHandler
    {
      public override double GetPower()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("CurrentGeneration").ToString(), out results);
        return results;
      }
    }

    // RTG
    public class ModuleRadioisotopeGeneratorHandler: PowerHandler
    {
      public override double GetPower()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("ActualPower").ToString(), out results);
        return results;
      }
    }

    // CryoTank
    public class ModuleCryoTankHandler: PowerHandler
    {
      public override double GetPower()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("currentCoolingCost").ToString(), out results);
        return results;
      }
    }

    // Antimatter Tank
    public class ModuleAntimatterTankHandler: PowerHandler
    {
      public override double GetPower()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("ContainmentCostCurrent").ToString(), out results);
        return results;
      }
    }

    // Chargeable Engine
    public class ModuleChargeableEngineHandler: PowerHandler
    {
      public override double GetPower()
      {
        double results = 0d;
        double.TryParse(pm.Fields.GetValue("currentCoolingCost").ToString(), out results);
        return results;
      }
    }
}
