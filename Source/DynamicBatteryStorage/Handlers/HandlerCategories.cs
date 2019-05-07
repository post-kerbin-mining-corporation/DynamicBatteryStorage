using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{
  public static class HandlerCategories
  {
    // This dictionary maps code names of UI categories to module names
    public static Dictionary<string, List<string>> HandlerCategoryMap = new Dictionary<string, List<string>>()
    {
      {"SolarPanels", new List<string>(new string[] {"ModuleDeployableSolarPanel", "ModuleCurvedSolarPanel", "KopernicusSolarPanel"})},
      {"Radiators", new List<string>(new string[] {"ModuleActiveRadiator"})},
      {"Converters", new List<string>(new string[] {"ModuleResourceConverter"})},
      {"Harvesters", new List<string>(new string[] {"ModuleResourceHarvester"})},
      {"Generators", new List<string>(new string[] {"ModuleGenerator", "ModuleCurvedSolarPanel", "ModuleRadioisotopeGenerator"})},
      {"FissionReactors", new List<string>(new string[] {"FissionGenerator"})},
      {"FusionReactors", new List<string>(new string[] {"ModuleDeployableSolarPanel", "ModuleCurvedSolarPanel", "KopernicusSolarPanel"})},
      {"Batteries", new List<string>(new string[] {"RealBattery"})},
      {"FuelTanks", new List<string>(new string[] {"ModuleCryoTank", "ModuleAntimatterTank"})},
    };


  // This enum maps code neames of all UI categories to localized version
  public static Dictionary<string, string> HandlerLocalizedNames = new Dictionary<string, string>()
  {
    {"SolarPanels", "Solar Panels"},
    {"Radiators", "Converters"},
    {"Converters", ""},
    {"Harvesters", "Harvesters"},
    {"Generators", "Generators"},
    {"FissionReactors", "Fission Reactors"},
    {"FusionReactors", "Fusion Reactors"},
    {"Batteries","Batteries" },
    {"FuelTanks", "Fuel Tanks"},
  };

}
