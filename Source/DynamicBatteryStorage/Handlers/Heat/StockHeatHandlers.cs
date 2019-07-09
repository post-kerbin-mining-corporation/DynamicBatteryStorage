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

    public ModuleActiveRadiatorHeatHandler()
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
      radiator = (ModuleActiveRadiator)pm;
      base.Initialize(pm);
      return true;
    }

    protected override double GetValueEditor()
    {
      if (radiator != null)
      {
        return -radiator.maxEnergyTransfer / 50d;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (radiator != null)
      {
        if (radiator.IsCooling)
          return -radiator.maxEnergyTransfer / 50d;
      }
      return 0d;
    }
  }

  // Resource Harvester
  public class ModuleResourceHarvesterHeatHandler : ModuleDataHandler
  {

    ModuleResourceHarvester harvester;
    ModuleCoreHeat core;

    public ModuleResourceHarvesterHeatHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = true;
      consumer = false;
    }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      harvester = (ModuleResourceHarvester)pm;
      core = pm.GetComponent<ModuleCoreHeat>();
      return harvester.GeneratesHeat;
    }

    protected override double GetValueEditor()
    {
      if (harvester != null)
      {
        // In editor, calculate predicted thermal production by using goal core temperature
        if (core != null)
          return (double)harvester.TemperatureModifier.Evaluate((float)core.CoreTempGoal) / 50d;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (harvester != null)
      {
        if (harvester.IsActivated)
          return harvester.lastHeatFlux;
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

    public ModuleResourceConverterHeatHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = true;
      consumer = false;
    }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      converter = (ModuleResourceConverter)pm;
      core = pm.GetComponent<ModuleCoreHeat>();
      return converter.GeneratesHeat;
    }

    protected override double GetValueEditor()
    {
      if (converter != null)
      {
        // In editor, calculate predicted thermal production by using goal core temperature
        if (core != null)
          return (double)converter.TemperatureModifier.Evaluate((float)core.CoreTempGoal) / 50f;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (converter != null)
      {
        if (converter.IsActivated)
          return converter.lastHeatFlux;
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return String.Format("{0} ({1})", base.PartTitle(), converter.ConverterName);
    }
  }

  public class ModuleCoreHeatHeatHandler : ModuleDataHandler
  {
    ModuleCoreHeat core;

    public ModuleCoreHeatHeatHandler()
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = true;
      consumer = false;
    }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      core = (ModuleCoreHeat)pm;

      if (core.MaxCoolant > 0f)
        return false;
      else
        return true;
    }

    protected override double GetValueEditor()
    {
      if (core != null)
      {
        // This is hard coded for now
        return (double)core.PassiveEnergy.Evaluate(300f) / 50f;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (core != null)
      {
        return (double)core.PassiveEnergy.Evaluate((float)core.CoreTemperature) / 50f;
      }
      return 0d;
    }
  }


}
