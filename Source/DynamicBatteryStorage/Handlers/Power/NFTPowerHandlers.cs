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
    public ModuleCurvedSolarPanelPowerHandler()
    {
      solarEfficiencyEffects = true;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = true;
    }
    protected override double GetValueEditor()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("TotalEnergyRate").ToString(), out results);
      return results * solarEfficiency;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("energyFlow").ToString(), out results);
      return results;
    }
  }

  // Fission Reactor
  public class FissionGeneratorPowerHandler: ModuleDataHandler
  {
    public FissionGeneratorPowerHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = true;
    }

    PartModule core;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      for (int i = 0;i<pm.part.Modules.Count;  i++)
      {
        if (pm.part.Modules[i].moduleName == "FissionReactor")
        {
          core = pm.part.Modules[i];
        }
      }
      return true;
    }

    protected override double GetValueEditor()
    {
      double results = 0d;
      float throttle = 100f;
      // Ensure we respect reactor throttle
      float.TryParse(core.Fields.GetValue("CurrentPowerPercent").ToString(), out throttle);
      double.TryParse(pm.Fields.GetValue("PowerGeneration").ToString(), out results);
      results = throttle / 100f * results;

      return results;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("CurrentGeneration").ToString(), out results);
      return results;
    }
  }

  // RTG
  public class ModuleRadioisotopeGeneratorPowerHandler: ModuleDataHandler
  {
    public ModuleRadioisotopeGeneratorPowerHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = true;
    }
    protected override double GetValueEditor()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("BasePower").ToString(), out results);
      return results;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("ActualPower").ToString(), out results);
      return results;
    }
  }

  // CryoTank
  public class ModuleCryoTankPowerHandler: ModuleDataHandler
  {
    public ModuleCryoTankPowerHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = false;
      consumer = true;
    }
    protected override double GetValueEditor()
    {
      double resAmt = GetMaxFuelAmt();
      double results = 0d;
      if (resAmt > 0d)
      {
        double.TryParse(pm.Fields.GetValue("CoolingCost").ToString(), out results);
        return results * (resAmt / 1000d) * -1d;
      } else
      {
        visible = false;
      }
      return results;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("currentCoolingCost").ToString(), out results);
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
  }

  // Antimatter Tank
  public class ModuleAntimatterTankPowerHandler: ModuleDataHandler
  {
    public ModuleAntimatterTankPowerHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = false;
      consumer = true;
    }
    protected override double GetValueEditor()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("ContainmentCost").ToString(), out results);
      return results* -1.0d;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("ContainmentCostCurrent").ToString(), out results);
      return results* -1.0d;
    }
  }

  // Chargeable Engine
  public class ModuleChargeableEnginePowerHandler: ModuleDataHandler
  {

    bool hasOfflineGenerator = false;
    double offlineBaseRate = 0d;
    public ModuleChargeableEnginePowerHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = false;
      consumer = true;
    }
    public override bool Initialize(PartModule pm)
    {
      bool.TryParse(pm.Fields.GetValue("PowerGeneratedOffline").ToString(), out hasOfflineGenerator);
      if (hasOfflineGenerator)
      {
        // NOTE: NEEDS FFT UPDATE
        double.TryParse(pm.Fields.GetValue("PowerGenerationTotal").ToString(), out offlineBaseRate);
        producer = true;
      }
      base.Initialize(pm);
      return true;
    }

    protected override double GetValueEditor()
    {
      double results = 0d;
      bool charging = false;

      bool.TryParse(pm.Fields.GetValue("Charging").ToString(), out charging);

      if (charging)
      {
        double.TryParse(pm.Fields.GetValue("ChargeRate").ToString(), out results);
        results *= -1d;
      }
      if (!charging && hasOfflineGenerator)
      {
        float genRate = 100f;
        float.TryParse(pm.Fields.GetValue("GeneratorRate").ToString(), out genRate);
        results = offlineBaseRate * genRate/100f;
      }
      return results;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      bool charging = false;

      bool.TryParse(pm.Fields.GetValue("Charging").ToString(), out charging);

      if (charging)
      {
          double.TryParse(pm.Fields.GetValue("ChargeRate").ToString(), out results);
          results *= -1d;
      }  else
      {
        double.TryParse(pm.Fields.GetValue("PowerGenerationTotal").ToString(), out results);
        return results;
      }
    }
  }

  // Centrifuge
  public class ModuleDeployableCentrifugePowerHandler : ModuleDataHandler
  {
    public ModuleDeployableCentrifugePowerHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = false;
      consumer = true;
    }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("SpinResourceRate").ToString(), out results);
      return results != 0d;
    }

    protected override double GetValueEditor()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue("SpinResourceRate").ToString(), out results);
      return -results;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      bool on = false;

      bool.TryParse(pm.Fields.GetValue("Rotating").ToString(), out on);
      if (on)
        double.TryParse(pm.Fields.GetValue("SpinResourceRate").ToString(), out results);
      return -results;
    }
  }
}
