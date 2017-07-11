using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

    // Deployable Solar Panel
    public class ModuleDeployableSolarPanelHandler: PowerHandler
    {
      ModuleDeployableSolarPanel panel;

      public override void Initialize(PartModule pm)
      {
        panel = (ModuleDeployableSolarPanel)pm;
      }

      public override double GetPower()
      {
        if (panel != null)
          return (double)panel.flowRate;
        return 0d;
      }
    }

    // Basic Generator
    public class ModuleGeneratorHandler: PowerHandler
    {
      ModuleGenerator gen;
      bool producer = false;
      double savedRate = 0.0;
      public override void Initialize(PartModule pm)
      {
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
              savedRate = gen.resHandler.inputResources[i].rate;
            }

      }
      public override double GetPower()
      {
        if (gen == null || !gen.generatorIsActive)
            return 0d;

        if (producer)
            return (double)gen.efficiency * savedRate;
        else
            return (double)gen.efficiency * savedRate * -1.0d;

      }
    }

    // Active Radiator
    public class ModuleActiveRadiatorHandler: PowerHandler
    {
      ModuleActiveRadiator radiator;

      public override void Initialize(PartModule pm)
      {
        panel = (ModuleActiveRadiator)pm;
      }

      public override double GetPower()
      {
        if (radiator == null || !radiator.isEnabled)
            return 0d;
        for (int i = 0; i < radiator.resHandler.inputResources.Count; i++)
        {
            if (radiator.resHandler.inputResources[i].name == "ElectricCharge")
                return radiator.resHandler.inputResources[i].rate * -1.0d;
        }
        return 0d;
      }
    }

    // Resource Harvester
    public class ModuleResourceHarvesterHandler: PowerHandler
    {

      ModuleResourceHarvester harvester;
      double converterEcRate = 0.0d;

      public override void Initialize(PartModule pm)
      {
        harvester = (ModuleResourceHarvester)pm;
        for (int i = 0; i < harvester.inputList.Count; i++)
        {
            if (harvester.inputList[i].ResourceName == "ElectricCharge")
            {
                converterEcRate = harvester.inputList[i].Ratio;
            }
          }
      }

      public override double GetPower()
      {
        if (harvester == null || !harvester.IsActivated)
            return 0d;
        //Debug.Log(harvester.lastTimeFactor);
        return converterEcRate * harvester.lastTimeFactor * -1.0d;
      }
    }

    // Resource Converter
    public class ModuleResourceConverterHandler: PowerHandler
    {
      ModuleResourceConverter converter;
      bool producer = false;
      double converterEcRate;

      public override void Initialize(PartModule pm)
      {
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

      public override double GetPower()
      {
        if (converter == null || !converter.IsActivated)
          return 0d;

        if (producer)
          return converterEcRate * converter.lastTimeFactor;
        else
          return converterEcRate * converter.lastTimeFactor * -1.0d;
      }
    }


}
