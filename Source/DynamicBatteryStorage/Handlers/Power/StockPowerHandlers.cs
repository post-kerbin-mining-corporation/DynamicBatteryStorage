using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

  /// <summary>
  /// Deployable Solar Panel
  /// </summary>
  public class ModuleDeployableSolarPanelPowerHandler : ModuleDataHandler
  {
    ModuleDeployableSolarPanel panel;
    public ModuleDeployableSolarPanelPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      panel = (ModuleDeployableSolarPanel)pm;
      return true;
    }

    protected override double GetValueEditor()
    {
      if (panel != null)
        return (double)panel.chargeRate * solarEfficiency;
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (panel != null)
        return (double)panel.flowRate;
      return 0d;
    }
  }

  /// <summary>
  /// ModuleCommand
  /// </summary>
  public class ModuleCommandPowerHandler : ModuleDataHandler
  {
    ModuleCommand pod;

    public ModuleCommandPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      pod = (ModuleCommand)pm;
      // Test to see if the ModuleCommand actually uses power
      for (int i = 0; i < pod.resHandler.inputResources.Count; i++)
      {
        if (pod.resHandler.inputResources[i].name == "ElectricCharge")
        {
          if (pod.resHandler.inputResources[i].rate > 0.0d)
          {
            return true;
          }
        }
      }
      return false;
    }

    protected override double GetValueEditor()
    {
      if (pod != null)
      {
        for (int i = 0; i < pod.resHandler.inputResources.Count; i++)
        {
          if (pod.resHandler.inputResources[i].name == "ElectricCharge")
          {
            return -pod.resHandler.inputResources[i].rate;
          }
        }
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (pod != null && !pod.IsHibernating)
      {
        for (int i = 0; i < pod.resHandler.inputResources.Count; i++)
        {
          if (pod.resHandler.inputResources[i].name == "ElectricCharge")
          {
            return -pod.resHandler.inputResources[i].rate;
          }
        }
      }
      return 0d;
    }
  }

  /// <summary>
  /// ModuleLight
  /// </summary>
  public class ModuleLightPowerHandler : ModuleDataHandler
  {
    ModuleLight light;

    public ModuleLightPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      light = (ModuleLight)pm;
      return true;
    }

    protected override double GetValueFlight()
    {
      if (light != null)
        return -1.0d * light.resourceAmount;
      return 0d;
    }
    protected override double GetValueEditor()
    {
      if (light != null)
        return -1.0d * light.resourceAmount;
      return 0d;
    }
  }

  /// <summary>
  /// ModuleDataTransmitter
  /// </summary>
  public class ModuleDataTransmitterPowerHandler : ModuleDataHandler
  {
    ModuleDataTransmitter antenna;

    public ModuleDataTransmitterPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      antenna = (ModuleDataTransmitter)pm;
      if (antenna.antennaType == AntennaType.INTERNAL)
        return false;

      return true;
    }

    protected override double GetValueEditor()
    {
      if (antenna != null)
        return -antenna.DataResourceCost * (1.0d / antenna.packetInterval);
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (antenna != null)
      {
        if (antenna.IsBusy())
          return -antenna.DataResourceCost * (1.0d / antenna.packetInterval);
      }
      return 0d;
    }
  }

  /// <summary>
  /// ModuleGenerator
  /// </summary>
  public class ModuleGeneratorPowerHandler : ModuleDataHandler
  {
    ModuleGenerator gen;
    double savedRate = 0.0;

    public ModuleGeneratorPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      gen = (ModuleGenerator)pm;
      bool toMonitor = false;
      for (int i = 0; i < gen.resHandler.inputResources.Count; i++)
        if (gen.resHandler.inputResources[i].name == "ElectricCharge")
        {
          producer = false;
          consumer = true;
          savedRate = gen.resHandler.inputResources[i].rate;
          toMonitor = true;
        }

      for (int i = 0; i < gen.resHandler.outputResources.Count; i++)
        if (gen.resHandler.outputResources[i].name == "ElectricCharge")
        {
          producer = true;
          consumer = false;
          savedRate = gen.resHandler.outputResources[i].rate;
          toMonitor = true;
        }
      return toMonitor;

    }
    protected override double GetValueEditor()
    {
      if (gen != null)
      {
        if (Producer)
          return savedRate;
        else
          return savedRate * -1.0d;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (gen != null)
      {
        if (gen.generatorIsActive)
        {
          if (producer)
            return gen.efficiency * savedRate;
          else
            return gen.efficiency * savedRate * -1.0d;
        }
      }
      return 0d;
    }
  }

  /// <summary>
  /// ModuleActiveRadiator
  /// </summary>
  public class ModuleActiveRadiatorPowerHandler : ModuleDataHandler
  {
    ModuleActiveRadiator radiator;

    public ModuleActiveRadiatorPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      radiator = (ModuleActiveRadiator)pm;
      return true;
    }

    protected override double GetValueEditor()
    {
      if (radiator != null)
      {
        for (int i = 0; i < radiator.resHandler.inputResources.Count; i++)
        {
          if (radiator.resHandler.inputResources[i].name == "ElectricCharge")
            return radiator.resHandler.inputResources[i].rate * -1.0d;
        }
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (radiator != null)
      {
        if (radiator.IsCooling)
        {
          for (int i = 0; i < radiator.resHandler.inputResources.Count; i++)
          {
            if (radiator.resHandler.inputResources[i].name == "ElectricCharge")
              return radiator.resHandler.inputResources[i].rate * -1.0d;
          }
        }
      }
      return 0d;
    }
  }

  /// <summary>
  /// ModuleResourceHarvester
  /// </summary>
  public class ModuleResourceHarvesterPowerHandler : ModuleDataHandler
  {

    ModuleResourceHarvester harvester;
    double converterEcRate = 0.0d;

    public ModuleResourceHarvesterPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      harvester = (ModuleResourceHarvester)pm;
      bool toMonitor = false;
      for (int i = 0; i < harvester.inputList.Count; i++)
      {
        if (harvester.inputList[i].ResourceName == "ElectricCharge")
        {
          converterEcRate = harvester.inputList[i].Ratio;
          toMonitor = true;
        }
      }
      return toMonitor;
    }

    protected override double GetValueEditor()
    {
      if (harvester != null)
      {
        return -converterEcRate;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (harvester != null)
      {
        if (harvester.IsActivated)
          return converterEcRate * harvester.lastTimeFactor * -1.0d;
      }
      return 0d;
    }

    public override string PartTitle()
    {
      return String.Format("{0} ({1})", base.PartTitle(), harvester.ConverterName);
    }
  }

  /// <summary>
  /// ModuleResourceConverter
  /// </summary>
  public class ModuleResourceConverterPowerHandler : ModuleDataHandler
  {
    ModuleResourceConverter converter;
    double converterEcRate = 0d;

    public ModuleResourceConverterPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      converter = (ModuleResourceConverter)pm;
      bool toMonitor = false;
      for (int i = 0; i < converter.inputList.Count; i++)
      {
        if (converter.inputList[i].ResourceName == "ElectricCharge")
        {
          converterEcRate = converter.inputList[i].Ratio;
          producer = false;
          consumer = true;
          toMonitor = true;
        }
      }
      for (int i = 0; i < converter.outputList.Count; i++)
      {
        if (converter.outputList[i].ResourceName == "ElectricCharge")
        {
          converterEcRate = converter.outputList[i].Ratio;
          producer = true;
          consumer = false;
          toMonitor = true;
        }
      }
      return toMonitor;
    }

    protected override double GetValueEditor()
    {
      if (converter != null)
      {
        if (producer)
          return converterEcRate;
        else
          return -converterEcRate;
      }
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (converter != null)
      {
        if (converter.IsActivated)
          if (producer)
            return converterEcRate * converter.lastTimeFactor;
          else
            return converterEcRate * converter.lastTimeFactor * -1.0d;
      }
      return 0d;
    }

    public override string PartTitle()
    {
      return String.Format("{0} ({1})", base.PartTitle(), converter.ConverterName);
    }
  }

  /// <summary>
  /// ModuleEngines
  /// </summary>
  public class ModuleEnginesPowerHandler : ModuleDataHandler
  {
    ModuleEngines engine;
    double engineBaseECRate = 0d;

    public ModuleEnginesPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      engine = (ModuleEngines)pm;

      bool toMonitor = false;
      double ecRatio = 1.0;
      double massFlowSum = 0d;
      double ratioSum = 0d;
      double totalRate = 0d;
      double massFlowTotal = engine.maxThrust / (9.81 * engine.atmosphereCurve.Evaluate(0f));
      // Computes the base electrical rate in seconds from the propellant ratios
      for (int i = 0; i < engine.propellants.Count; i++)
      {
        if (engine.propellants[i].name == "ElectricCharge")
        {
          toMonitor = true;
          ecRatio = engine.propellants[i].ratio;
        }
        else
        {
          ratioSum += engine.propellants[i].ratio;
          massFlowSum += engine.propellants[i].ratio * PartResourceLibrary.Instance.GetDefinition(engine.propellants[i].name).density;
        }
      }
      double mixtureRatio = massFlowSum / ratioSum;

      for (int i = 0; i < engine.propellants.Count; i++)
      {
        if (engine.propellants[i].name != "ElectricCharge")
          totalRate += (massFlowTotal / mixtureRatio) * engine.propellants[i].ratio / ratioSum;
      }
      engineBaseECRate = ecRatio / ratioSum * totalRate;
      return toMonitor;
    }

    protected override double GetValueEditor()
    {
      if (engine != null)
      {
        float throttle = engine.thrustPercentage / 100f;
        return -engineBaseECRate * throttle;
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (engine != null)
      {
        if (!engine.engineShutdown)
        {
          return -engineBaseECRate * engine.currentThrottle;
        }
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return String.Format("{0} ({1})", base.PartTitle(), engine.engineID);
    }
  }

  public class ModuleEnginesFXPowerHandler : ModuleEnginesPowerHandler
  {
    public ModuleEnginesFXPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
  }

  /// <summary>
  /// ModuleAlternator
  /// </summary>
  public class ModuleAlternatorPowerHandler : ModuleDataHandler
  {
    ModuleAlternator alternator;
    ModuleEngines[] engines;

    public ModuleAlternatorPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      alternator = (ModuleAlternator)pm;
      engines = pm.part.GetComponents<ModuleEngines>();

      // Test to see if the ModuleCommand actually uses power
      for (int i = 0; i < alternator.resHandler.outputResources.Count; i++)
      {
        if (alternator.resHandler.outputResources[i].name == "ElectricCharge")
        {
          if (alternator.resHandler.outputResources[i].rate > 0.0d)
          {
            return true;
          }
        }
      }

      return false;
    }

    protected override double GetValueEditor()
    {
      if (alternator != null)
      {
        for (int i = 0; i < alternator.resHandler.outputResources.Count; i++)
        {
          if (alternator.resHandler.outputResources[i].name == "ElectricCharge")
          {
            return alternator.resHandler.outputResources[i].rate;
          }
        }
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (alternator != null && engines != null)
      {
        if (alternator.preferMultiMode)
        {
          foreach (ModuleEngines e in engines)
          {
            if (e.isOperational)
            {
              return alternator.outputRate;
            }
          }
        }
        else
        {
          foreach (ModuleEngines e in engines)
          {
            if (e.isOperational)
            {
              return alternator.outputRate;
            }
            else
              return 0f;
          }
        }
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return String.Format("{0} (Alternator)", base.PartTitle());
    }
  }

  /// <summary>
  /// ModuleScienceLab
  /// </summary>
  public class ModuleScienceLabPowerHandler : ModuleDataHandler
  {
    ModuleScienceLab lab;
    double processRate = 0d;
    public ModuleScienceLabPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      lab = (ModuleScienceLab)pm;
      return true;
    }

    protected override double GetValueEditor()
    {
      if (lab != null)
      {

        for (int i = 0; i < lab.processResources.Count; i++)
        {
          if (lab.processResources[i].name == "ElectricCharge")
          {
            processRate = -1.0d * lab.processResources[i].amount;
            return processRate;
          }
        }
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (lab != null)
      {
        if (lab.IsOperational())
          return processRate;
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return String.Format("{0} (Clean Experiments)", base.PartTitle());
    }
  }
  /// <summary>
  /// ModuleAlternator
  /// </summary>
  public class ModuleScienceConverterPowerHandler : ModuleDataHandler
  {
    ModuleScienceConverter lab;
    double processRate = 0d;
    public ModuleScienceConverterPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      lab = (ModuleScienceConverter)pm;
      return true;
    }

    protected override double GetValueEditor()
    {
      if (lab != null)
      {

        return -1.0d * lab.powerRequirement;
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (lab != null)
      {
        if (lab.ModuleIsActive())
        {
          return -1.0d * lab.powerRequirement;
        }
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return String.Format("{0} (Science Lab)", base.PartTitle());
    }
  }
}
