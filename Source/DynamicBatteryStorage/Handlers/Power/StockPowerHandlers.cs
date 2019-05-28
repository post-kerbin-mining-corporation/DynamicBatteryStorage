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

    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      panel = (ModuleDeployableSolarPanel)pm;
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
  }

  /// <summary>
  /// ModuleCommand
  /// </summary>
  public class ModuleCommandPowerHandler : ModuleDataHandler
  {
    ModuleCommand pod;

    bool producer = false;
    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      pod = (ModuleCommand)pm;
    }

    public override double GetValue()
    {

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
            return pod.resHandler.inputResources[i].rate;
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

    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      light = (ModuleLight)pm;
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

    public override void Initialize(PartModule pm)
    {
      base.Initialize(pm);
      antenna = (ModuleDataTransmitter)pm;
    }
    public override bool IsProducer()
    {
      return false;
    }
    public override double GetValue()
    {

      if (antenna != null)
      {
        return antenna.DataResourceCost * (1.0d / antenna.packetInterval);

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
      public override void Initialize(PartModule pm)
      {
         base.Initialize(pm);
        gen = (ModuleGenerator)pm;

        for (int i = 0; i < gen.resHandler.inputResources.Count; i++)
            if (gen.resHandler.inputResources[i].name == "ElectricCharge")
            {
              producer = false;
              savedRate = gen.resHandler.inputResources[i].rate;
            }

        for (int i = 0; i < gen.resHandler.outputResources.Count; i++)
            if (gen.resHandler.outputResources[i].name == "ElectricCharge")
            {
              producer = true;
              savedRate = gen.resHandler.outputResources[i].rate;
            }

      }
      public override double GetValue()
      {
        if (gen == null || !gen.generatorIsActive)
            return 0d;

        if (producer)
            return (double)gen.efficiency * savedRate;
        else
            return (double)gen.efficiency * savedRate * -1.0d;

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

      public override void Initialize(PartModule pm)
      {
          base.Initialize(pm);
        radiator = (ModuleActiveRadiator)pm;
      }

      public override double GetValue()
      {
        if (radiator == null || !radiator.IsCooling)
            return 0d;
        for (int i = 0; i < radiator.resHandler.inputResources.Count; i++)
        {
            if (radiator.resHandler.inputResources[i].name == "ElectricCharge")
                return radiator.resHandler.inputResources[i].rate * -1.0d;
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

      public override void Initialize(PartModule pm)
      {
        base.Initialize(pm);
        harvester = (ModuleResourceHarvester)pm;
        for (int i = 0; i < harvester.inputList.Count; i++)
        {
            if (harvester.inputList[i].ResourceName == "ElectricCharge")
            {
                converterEcRate = harvester.inputList[i].Ratio;
            }
          }
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
          return String.Format("{0} ({1})", base.PartTitle + harvester.ConverterName);
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

      public override void Initialize(PartModule pm)
      {
        base.Initialize(pm);
        converter = (ModuleResourceConverter)pm;

        for (int i = 0; i < converter.inputList.Count; i++)
        {
            if (converter.inputList[i].ResourceName == "ElectricCharge")
            {
                converterEcRate = converter.inputList[i].Ratio;
                producer = false;
              }
          }
          for (int i = 0; i < converter.outputList.Count; i++)
          {
              if (converter.outputList[i].ResourceName == "ElectricCharge")
              {
                  converterEcRate = converter.outputList[i].Ratio;
                  producer = true;
                }
            }
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
          return String.Format("{0} ({1})", base.PartTitle + converter.ConverterName);
      }

      public override bool IsProducer()
      {
          return producer;
      }
    }


}
