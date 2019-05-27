using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

  // Deployable Solar Panel
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

  // Module COmmand
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
      return 0d;
    }
    public override bool IsProducer()
    {
      return false;
    }
  }
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

  // Basic Generator
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

    // Active Radiator
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

    // Resource Harvester
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
        if (harvester == null || !harvester.IsActivated)
            return 0d;
        //Debug.Log(harvester.lastTimeFactor);
        return converterEcRate * harvester.lastTimeFactor * -1.0d;
      }
      public override bool IsProducer()
      {
          return false;
      }
    }

    // Resource Converter
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
        if (converter == null || !converter.IsActivated)
          return 0d;

        if (producer)
          return converterEcRate * converter.lastTimeFactor;
        else
          return converterEcRate * converter.lastTimeFactor * -1.0d;
      }

      public override bool IsProducer()
      {
          return producer;
      }
    }


}
