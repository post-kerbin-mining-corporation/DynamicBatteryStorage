using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{
    public enum PowerConsumerType {
      ModuleActiveRadiator,
      ModuleResourceHarvester,
      ModuleGenerator,
      ModuleResourceConverter,
      ModuleCryoTank
    }

    public class PowerConsumer
    {

      PowerConsumerType consumerType;
      // Generic reference to PartModule
      PartModule pm;

      // Hard references to stock modules
      ModuleGenerator gen;
      ModuleResourceConverter converter;
      ModuleResourceHarvester harvester;
      ModuleActiveRadiator radiator;

      double converterEcRate;

      public string ConsumerType { get { return consumerType.ToString(); } }

      public PowerConsumer(PowerConsumerType tp, PartModule mod)
      {
        consumerType = tp;
        pm = mod;
        switch (consumerType)
        {
          case PowerConsumerType.ModuleActiveRadiator:
            radiator = (ModuleActiveRadiator)pm;
            break;
          case PowerConsumerType.ModuleResourceConverter:
            converter = (ModuleResourceConverter)pm;
            for (int i = 0; i < converter.resHandler.inputResources.Count; i++)
                if (converter.resHandler.inputResources[i].name == "ElectricCharge")
                    converterEcRate = converter.resHandler.inResources[i].rate;
            break;
          case PowerConsumerType.ModuleGenerator:
            gen = (ModuleGenerator)pm;
            break;
          case PowerConsumerType.ModuleResourceHarvester:
            harvester = (ModuleResourceHarvester)pm;
            break;
        }
      }
      public double GetPowerConsumption()
      {
        switch (consumerType)
        {
          case PowerConsumerType.ModuleActiveRadiator:
            return GetModuleActiveRadiatorConsumption();
          case PowerConsumerType.ModuleResourceConverter:
            return GetModuleResourceConverterConsumption();
          case PowerConsumerType.ModuleGenerator:
            return GetModuleGeneratorConsumption();
          case PowerConsumerType.ModuleResourceHarvester:
            return GetModuleResourceHarvesterConsumption();
          case PowerConsumerType.ModuleCryoTank:
            return GetModuleCryoTankConsumption();
        }
        return 0d;
      }

      double GetModuleGeneratorConsumption()
      {
          if (gen == null || !gen.generatorIsActive)
              return 0d;
          for (int i = 0; i < gen.resHandler.inputResources.Count; i++)
              if (gen.resHandler.inputResources[i].name == "ElectricCharge")
                  return (double)gen.efficiency * gen.resHandler.inputResources[i].rate;

          return 0d;
      }
      double GetModuleResourceConverterConsumption()
      {
          if (converter == null || !converter.IsActivated)
            return 0d;
          return converterEcRate * converter.lastTimeFactor;
      }
      double GetModuleResourceHarvesterConsumption()
      {
          return 0d;
      }
      double GetModuleActiveRadiatorConsumption()
      {
          //Debug.Log("query radiator");
          if (radiator == null || !radiator.isEnabled)
              return 0d;
          for (int i = 0; i < radiator.resHandler.inputResources.Count; i++)
          {
              if (radiator.resHandler.inputResources[i].name == "ElectricCharge")
                  return radiator.resHandler.inputResources[i].rate;
          }
          return 0d;
      }
      double GetModuleCommandConsumption()
      {
        return 0d;
      }
      double GetModuleCryoTankConsumption()
      {
        return 0d;
      }
    }
}
