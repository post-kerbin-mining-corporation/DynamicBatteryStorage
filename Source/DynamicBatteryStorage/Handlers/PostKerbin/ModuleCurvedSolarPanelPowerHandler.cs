namespace DynamicBatteryStorage
{

  public class ModuleCurvedSolarPanelPowerHandler : ModuleDataHandler
  {
    public ModuleCurvedSolarPanelPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      /// If kopernicus is using the multistar settings, we need to show this in the editor but NOT in flight
      if (Settings.Kopernicus && Settings.KopernicusMultiStar || Settings.WeatherDrivenSolarPanel)
      {
        return HighLogic.LoadedSceneIsEditor;
      }
      return true;
    }
    protected override double GetValueEditor()
    {
      Utils.TryGetField(pm, "TotalEnergyRate", out double results);
      return results * solarEfficiency;
    }
    protected override double GetValueFlight()
    {
      Utils.TryGetField(pm, "energyFlow", out double results);
      return results;
    }
  }

}
