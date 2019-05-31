using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{
  // This enum consists of all handled types. They MUST match the module name
  public enum PowerHandlerType {
    // Stock
    ModuleDeployableSolarPanel,
    ModuleGenerator,
    ModuleResourceConverter,
    ModuleActiveRadiator,
    ModuleResourceHarvester,
    ModuleCommand,
    ModuleLight,
    ModuleDataTransmitter,
    ModuleEnginesFX,

    // NFT
    ModuleCurvedSolarPanel,
    FissionGenerator,
    ModuleRadioisotopeGenerator,
    ModuleCryoTank,
    ModuleAntimatterTank,
    ModuleDeployableCentrifuge,

    // RealBattery
    RealBattery,

    // Kopernicus
    KopernicusSolarPanel,

    // SSTU
    SSTUResourceBoiloff,
    SSTUSolarPanelStatic,
    SSTUSolarPanelDeployable

  }

  // This enum consists of all handled types. They MUST match the module name
  public enum HeatHandlerType {
    // Stock
    ModuleCoreHeat,       // ONLY HANDLES PASSIVE HEAT
    ModuleResourceConverter,
    ModuleActiveRadiator,
    ModuleResourceHarvester,

    // NFT
    FissionReactor
  }
}
