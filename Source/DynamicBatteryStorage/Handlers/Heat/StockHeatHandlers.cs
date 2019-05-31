using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

  // Active Radiator
  public class ModuleActiveRadiatorHeatHandler : ModuleDataHandler
  {
    ModuleActiveRadiator radiator;

    public override bool Initialize(PartModule pm)
    {
      radiator = (ModuleActiveRadiator)pm;
      return base.Initialize(pm);
    }

    public override double GetValue()
    {
      if (radiator == null)
        return 0d;
      if (HighLogic.LoadedSceneIsEditor)

        return -radiator.maxEnergyTransfer / 50d;
      if (HighLogic.LoadedSceneIsFlight && radiator.IsCooling)
        return -radiator.maxEnergyTransfer / 50d;
      return 0d;
    }
    public override bool IsProducer()
    {
      return false;
    }
  }

  // Resource Harvester
  public class ModuleResourceHarvesterHeatHandler : ModuleDataHandler
  {

    ModuleResourceHarvester harvester;
    ModuleCoreHeat core;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      harvester = (ModuleResourceHarvester)pm;
      core = pm.GetComponent<ModuleCoreHeat>();
      return harvester.GeneratesHeat;
    }

    public override double GetValue()
    {
      if (harvester == null)
        return 0d;

      visible = harvester.GeneratesHeat;
      
      if (HighLogic.LoadedSceneIsFlight)
      {
        if (harvester.IsActivated)
          return harvester.lastHeatFlux;
      }
      if (HighLogic.LoadedSceneIsEditor)
      {
        // In editor, calculate predicted thermal draw by using goal core temperature
        if (core != null)
          return (double)harvester.TemperatureModifier.Evaluate((float)core.CoreTempGoal) / 50d;
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return String.Format("{0} ({1})", base.PartTitle(), harvester.ConverterName);
    }
  }

  // Resource Converter
  public class ModuleResourceConverterHeatHandler : ModuleDataHandler
  {
    ModuleResourceConverter converter;
    ModuleCoreHeat core;
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      converter = (ModuleResourceConverter)pm;
      core = pm.GetComponent<ModuleCoreHeat>();
      return converter.GeneratesHeat;
    }

    public override double GetValue()
    {
      if (converter == null)
        return 0d;

      visible = converter.GeneratesHeat;

      if (HighLogic.LoadedSceneIsFlight)
      {
        if (converter.IsActivated)
          return converter.lastHeatFlux;
      }
      if (HighLogic.LoadedSceneIsEditor)
      {
        // In editor, calculate predicted thermal draw by using goal core temperature
        if (core != null)
          return (double)converter.TemperatureModifier.Evaluate((float)core.CoreTempGoal) / 50f;
      }
      return 0d;
    }
    public override bool IsProducer()
    {
      return true;
    }
    public override string PartTitle()
    {
      return String.Format("{0} ({1})", base.PartTitle(), converter.ConverterName);
    }
  }
  public class ModuleCoreHeatHeatHandler : ModuleDataHandler
  {
    ModuleCoreHeat core;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      core = (ModuleCoreHeat)pm;
      return true;
    }

    public override double GetValue()
    {
      if (core == null)
        return 0d;
      if (HighLogic.LoadedSceneIsEditor)
        return (double)core.PassiveEnergy.Evaluate(300f) / 50f;
      else
        return (double)core.PassiveEnergy.Evaluate((float)core.CoreTemperature) / 50f;
    }
    public override bool IsProducer()
    {
      return true;
    }

  }


}
