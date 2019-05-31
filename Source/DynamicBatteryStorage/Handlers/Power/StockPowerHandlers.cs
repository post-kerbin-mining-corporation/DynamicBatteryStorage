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
  public class ModuleDeployableSolarPanelPowerHandler: ModuleDataHandler
  {
    ModuleDeployableSolarPanel panel;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      panel = (ModuleDeployableSolarPanel)pm;
      return true;
    }

    public override double GetValue()
    {

      if (panel != null)
      {
        if (HighLogic.LoadedSceneIsEditor)
          return (double)panel.chargeRate;
        return (double)panel.flowRate;
      }
      return 0d;
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

  /// <summary>
  /// ModuleCommand
  /// </summary>
  public class ModuleCommandPowerHandler : ModuleDataHandler
  {
    ModuleCommand pod;

    bool producer = false;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      pod = (ModuleCommand)pm;
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

    public override double GetValue()
    {
      visible = false;
      if (pod != null)
      {
        for (int i = 0; i < pod.resHandler.inputResources.Count; i++)
        {
          if (pod.resHandler.inputResources[i].name == "ElectricCharge")
          {
            if (pod.resHandler.inputResources[i].rate > 0.0d)
            {
              visible = true;
            } else
            {
              visible = false;
            }
            return -pod.resHandler.inputResources[i].rate;
          }
        }

      }
      return 0d;
    }
    public override bool IsProducer()
    {
      return false;
    }
  }

  /// <summary>
  /// ModuleLight
  /// </summary>
  public class ModuleLightPowerHandler : ModuleDataHandler
  {
    ModuleLight light;
    bool producer = false;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      light = (ModuleLight)pm;
      return true;
    }
    public override bool IsProducer()
    {
      return false;
    }
    public override double GetValue()
    {

      if (light != null)
      {
        return light.resourceAmount;

      }
      return 0d;
    }
  }

  /// <summary>
  /// ModuleDataTransmitter
  /// </summary>
  public class ModuleDataTransmitterPowerHandler : ModuleDataHandler
  {
    ModuleDataTransmitter antenna;
    bool producer = false;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      simulated = false;
      antenna = (ModuleDataTransmitter)pm;
      return true;
    }
    public override bool IsProducer()
    {
      return false;
    }
    public override double GetValue()
    {

      if (antenna != null)
      {
        return -antenna.DataResourceCost * (1.0d / antenna.packetInterval);

      }
      return 0d;
    }
  }

  /// <summary>
  /// ModuleGenerator
  /// </summary>
  public class ModuleGeneratorPowerHandler: ModuleDataHandler
  {
    ModuleGenerator gen;
    bool producer = false;
    double savedRate = 0.0;
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      gen = (ModuleGenerator)pm;
      bool toMonitor = false;
      for (int i = 0; i < gen.resHandler.inputResources.Count; i++)
      if (gen.resHandler.inputResources[i].name == "ElectricCharge")
      {
        producer = false;
        savedRate = gen.resHandler.inputResources[i].rate;
        toMonitor = true;
      }

      for (int i = 0; i < gen.resHandler.outputResources.Count; i++)
      if (gen.resHandler.outputResources[i].name == "ElectricCharge")
      {
        producer = true;
        savedRate = gen.resHandler.outputResources[i].rate;
        toMonitor = true;
       }
      return toMonitor;

    }
    public override double GetValue()
    {
      
      if (gen == null)
          return 0d;
      if (HighLogic.LoadedSceneIsEditor)
      {
        if (producer)
          return (double)savedRate;
        else
          return (double)savedRate * -1.0d;
      }
      if (HighLogic.LoadedSceneIsFlight)
      {
        if (!gen.generatorIsActive)
        {
          if (producer)
            return (double)gen.efficiency * savedRate;
          else
            return (double)gen.efficiency * savedRate * -1.0d;
        }
      }
      return 0d;

    }
    public override bool IsProducer()
    {
        return producer;
    }
  }

  /// <summary>
  /// ModuleActiveRadiator
  /// </summary>
  public class ModuleActiveRadiatorPowerHandler: ModuleDataHandler
  {
    ModuleActiveRadiator radiator;

    public override bool Initialize(PartModule pm)
    {
        base.Initialize(pm);
      radiator = (ModuleActiveRadiator)pm;
    return true;
    }

    public override double GetValue()
    {
      if (radiator == null)
          return 0d;
      if (HighLogic.LoadedSceneIsFlight && !radiator.IsCooling)
    {
      for (int i = 0; i < radiator.resHandler.inputResources.Count; i++)
      {
        if (radiator.resHandler.inputResources[i].name == "ElectricCharge")
          return radiator.resHandler.inputResources[i].rate * -1.0d;
      }
    }
    if (HighLogic.LoadedSceneIsEditor)
    {
      for (int i = 0; i < radiator.resHandler.inputResources.Count; i++)
      {
        if (radiator.resHandler.inputResources[i].name == "ElectricCharge")
          return radiator.resHandler.inputResources[i].rate * -1.0d;
      }
    }
      return 0d;
    }
    public override bool IsProducer()
    {
        return false;
    }
  }

    /// <summary>
    /// ModuleResourceHarvester
    /// </summary>
  public class ModuleResourceHarvesterPowerHandler: ModuleDataHandler
  {

    ModuleResourceHarvester harvester;
    double converterEcRate = 0.0d;

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

    public override double GetValue()
    {
      if (harvester == null)
          return 0d;
      if (HighLogic.LoadedSceneIsEditor)
      {
        return -converterEcRate;
      } else
      {
        if (harvester.IsActivated)
          return converterEcRate * harvester.lastTimeFactor * -1.0d;
        else
          return 0d;
      }
    }
    public override bool IsProducer()
    {
        return false;
    }
    public override string PartTitle()
    {
        return String.Format("{0} ({1})", base.PartTitle() , harvester.ConverterName);
    }
  }

  /// <summary>
  /// ModuleResourceConverter
  /// </summary>
  public class ModuleResourceConverterPowerHandler: ModuleDataHandler
  {
    ModuleResourceConverter converter;
    bool producer = false;
    double converterEcRate;

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
          toMonitor = true;
        }
      }
      for (int i = 0; i < converter.outputList.Count; i++)
      {
        if (converter.outputList[i].ResourceName == "ElectricCharge")
        {
          converterEcRate = converter.outputList[i].Ratio;
          producer = true;
          toMonitor = true;
        }
      }
      simulated = false;
     
      return toMonitor;
    }

    public override double GetValue()
    {
      if (converter == null)
        return 0d;

      if (HighLogic.LoadedSceneIsEditor)
      {
        if (producer)
          return converterEcRate;
        else
          return -converterEcRate;
      } else
      {
        if (converter.IsActivated)
        {
          if (producer)
            return converterEcRate * converter.lastTimeFactor;
          else
            return converterEcRate * converter.lastTimeFactor * -1.0d;
        }
        return 0d;
      }

    }

    public override string PartTitle()
    {
        return String.Format("{0} ({1})", base.PartTitle(), converter.ConverterName);
    }

    public override bool IsProducer()
    {
        return producer;
    }
  }

  /// <summary>
  /// ModuleResourceConverter
  /// </summary>
  public class ModuleEnginesFXPowerHandler : ModuleDataHandler
  {
    ModuleEnginesFX engine;
    double engineBaseECRate =0d;

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      engine = (ModuleEnginesFX)pm;
      bool toMonitor = false;
      double ecRatio = 1.0;

      double massFlowSum = 0d;
      double ratioSum = 0d;
      double totalRate = 0d;

      double massFlowTotal = engine.maxThrust / (9.81 * engine.atmosphereCurve.Evaluate(0f));

      for (int i = 0; i < engine.propellants.Count; i++)
      {
        if (engine.propellants[i].name == "ElectricCharge")
        {
          toMonitor = true;
          ecRatio = engine.propellants[i].ratio;
        } else
        {
          ratioSum += engine.propellants[i].ratio;
          massFlowSum += engine.propellants[i].ratio * PartResourceLibrary.Instance.GetDefinition(engine.propellants[i].name).density;
        }
      }
      double mixtureRatio = massFlowSum / ratioSum;

      for (int i = 0; i < engine.propellants.Count; i++)
      {
        if (engine.propellants[i].name == "ElectricCharge")
        {}
        else
        {
          totalRate += (massFlowTotal/mixtureRatio) * engine.propellants[i].ratio/ratioSum;
        }
      }
     
      engineBaseECRate = ecRatio / ratioSum * totalRate;
      
      return toMonitor;
    }

    public override double GetValue()
    {
      if (engine == null)
        return 0d;

      if (HighLogic.LoadedSceneIsEditor)
      {
        float throttle = engine.thrustPercentage / 100f;
        return -engineBaseECRate * throttle;
      }
      else
      {
        if (!engine.engineShutdown)
        {
          return -engineBaseECRate * engine.currentThrottle / 100f;
        }
      }
      return 0d;
    }

    public override string PartTitle()
    {
      return String.Format("{0} ({1})", base.PartTitle(), engine.engineID);
    }

    public override bool IsProducer()
    {
      return false;
    }
  }

}
