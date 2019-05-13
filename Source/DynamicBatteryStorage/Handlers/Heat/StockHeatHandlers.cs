using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

  // Active Radiator
  public class ModuleActiveRadiatorHeatHandler: ModuleDataHandler
  {
    ModuleActiveRadiator radiator;

    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      radiator = (ModuleActiveRadiator)pm;
    }

    public override double GetValue()
    {
      if (radiator == null || !radiator.IsCooling)
        return 0d;
      return radiator.maxEnergyTransfer/50d;
    }
    public override bool IsProducer()
    {
      return false;
    }
  }

  // Resource Harvester
  public class ModuleResourceHarvesterHeatHandler: ModuleDataHandler
  {

    ModuleResourceHarvester harvester;
    ModuleCoreHeat core;

    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      harvester = (ModuleResourceHarvester)pm;
      core = pm.GetComponent<ModuleCoreHeat>();
    }

    public override double GetValue()
    {
      if (harvester == null || !harvester.IsActivated)
        return 0d;
      if (HighLogic.LoadedSceneIsFlight)
        return harvester.lastHeatFlux;
      else
      {
        // In editor, calculate predicted thermal draw by using goal core temperature
        if (core != null)
          return (double)harvester.TemperatureModifier.Evaluate((float)core.CoreTempGoal) / 50f;

        return 0d;
      }
  }

  // Resource Converter
  public class ModuleResourceConverterHeatHandler: ModuleDataHandler
  {
    ModuleResourceConverter converter;
    ModuleCoreHeat core;
    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      converter = (ModuleResourceConverter)pm;
      core = pm.GetComponent<ModuleCoreHeat>();
    }

    public override double GetValue()
    {
      if (converter == null || !converter.IsActivated)
        return 0d;

      if (HighLogic.LoadedSceneIsFlight)
        return converter.lastHeatFlux;
      else
      {
        // In editor, calculate predicted thermal draw by using goal core temperature
        if (core != null)
          return (double)converter.TemperatureModifier.Evaluate((float)core.CoreTempGoal)/ 50f;

        return 0d;
      }
    }

  }
  public class ModuleCoreHeatHandler : ModuleDataHandler
  {
    ModuleCoreHeat core;

    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      core = (ModuleCoreHeat)pm;
    }

    public override double GetValue()
    {
      if (core == null)
        return 0d;
      if (HighLogic.LoadedSceneIsEditor)
        return (double)core.PassiveEnergy.Evaluate(300f)/50f;
      else
        return (double)core.PassiveEnergy.Evaluate((float)core.CoreTemperature)/50f;
    }

  }

}
