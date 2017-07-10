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
      ModuleCryoTank,
      ModuleAntimatterTank,
      RealBattery
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
            for (int i = 0; i < converter.inputList.Count; i++)
                if (converter.inputList[i].ResourceName == "ElectricCharge")
                    converterEcRate = converter.inputList[i].Ratio;
            break;
          case PowerConsumerType.ModuleGenerator:
            gen = (ModuleGenerator)pm;
            break;
          case PowerConsumerType.ModuleResourceHarvester:
            harvester = (ModuleResourceHarvester)pm;
            for (int i = 0; i < harvester.inputList.Count; i++)
                if (harvester.inputList[i].ResourceName == "ElectricCharge")
                    converterEcRate = harvester.inputList[i].Ratio;
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
          case PowerConsumerType.ModuleAntimatterTank:
              return GetModuleAntimatterTankConsumption();
          case PowerConsumerType.RealBattery:
              return GetModuleRealBatteryConsumption();
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
          if (harvester == null || !harvester.IsActivated)
              return 0d;
          //Debug.Log(harvester.lastTimeFactor);
          return converterEcRate * harvester.lastTimeFactor;
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
          double results = 0d;
          double.TryParse(pm.Fields.GetValue("currentCoolingCost").ToString(), out results);
          return results;
      }
      double GetModuleAntimatterTankConsumption()
      {
          double results = 0d;
          double.TryParse(pm.Fields.GetValue("ContainmentCostCurrent").ToString(), out results);
          return results;
      }
      // RealBattery
      double GetModuleRealBatteryConsumption()
      {
          double results = 0d;
          double.TryParse(pm.Fields.GetValue("lastECpower").ToString(), out results);
          return results > 0 ? results : 0d; // positive value means the battery is charging --> consuming EC
      }
    }
}
