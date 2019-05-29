using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;

namespace DynamicBatteryStorage
{
  public static class HandlerCategories
  {
    // This dictionary maps code names of UI categories to module names
    public static Dictionary<string, List<string>> HandlerCategoryMap = new Dictionary<string, List<string>>()
    {
      {"SolarPanels", new List<string>(new string[] {"ModuleDeployableSolarPanel", "ModuleCurvedSolarPanel", "KopernicusSolarPanel", "SSTUSolarPanelStatic", "SSTUSolarPanelDeployable"})},
      {"Radiators", new List<string>(new string[] {"ModuleActiveRadiator"})},
      {"Converters", new List<string>(new string[] {"ModuleResourceConverter"})},
      {"Harvesters", new List<string>(new string[] {"ModuleResourceHarvester"})},
      {"Generators", new List<string>(new string[] {"ModuleGenerator", "ModuleCurvedSolarPanel", "ModuleRadioisotopeGenerator"})},
      {"FissionReactors", new List<string>(new string[] {"FissionGenerator"})},
      {"FusionReactors", new List<string>(new string[] {"ModuleFusionReactor"})},
      {"Batteries", new List<string>(new string[] {"RealBattery"})},
      {"FuelTanks", new List<string>(new string[] {"ModuleCryoTank", "ModuleAntimatterTank", "SSTUResourceBoiloff"})},
      {"Lights", new List<string>(new string[] {"ModuleLight"})},
      {"Command", new List<string>(new string[] {"ModuleCommand"}) },
      {"Communication", new List<string>(new string[] {"ModuleDataTransmitter"}) }
    };


    // This enum maps code neames of all UI categories to localized version
    public static Dictionary<string, string> HandlerLocalizedNames = new Dictionary<string, string>()
    {
      {"SolarPanels", ""},
      {"Radiators", "Radiators"},
      {"Converters", "Converters"},
      {"Harvesters", "Harvesters"},
      {"Generators", "Generators"},
      {"FissionReactors", "Fission Reactors"},
      {"FusionReactors", "Fusion Reactors"},
      {"Batteries","Batteries" },
      {"FuelTanks", "Fuel Tanks"},
      {"Lights", "Lights" },
      {"Command", "Command Modules" },
      {"Communication", "Transmitters" }

    };
    public static LocalizeStrings()
    {
      HandlerLocalizedNames = new Dictionary<string, string>()
      {
        {"SolarPanels", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_SolarPanels")},
        {"Radiators", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_Radiators")},
        {"Converters", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_Converters")},
        {"Harvesters", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_Harvesters")},
        {"Generators", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_Generators")},
        {"FissionReactors", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_FissionReactors")},
        {"FusionReactors", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_FusionReactors")},
        {"Batteries", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_Batteries") },
        {"FuelTanks", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_FuelTanks")},
        {"Lights", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_Lights") },
        {"Command", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_Command") },
        {"Communication", Localizer.Format("#LOC_DynamicBatteryStorage_UI_Category_Communication") }

    }
  }
}
