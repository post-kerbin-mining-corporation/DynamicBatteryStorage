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
      double converterEcRate = 0.0d;

      public override void Initialize(PartModule pm)
      {
        base.Initialize(pm);
        harvester = (ModuleResourceHarvester)pm;

      }

      public override double GetValue()
      {
        if (harvester == null || !harvester.IsActivated)
          return 0d;
        return lastHeatFlux;
      }
    }

    // Resource Converter
    public class ModuleResourceConverterHeatHandler: ModuleDataHandler
    {
      ModuleResourceConverter converter;

      public override void Initialize(PartModule pm)
      {
        base.Initialize(pm);
        converter = (ModuleResourceConverter)pm;
      }

      public override double GetValue()
      {
        if (converter == null || !converter.IsActivated)
          return 0d;

        return lastHeatFlux;
      }

    }


}
