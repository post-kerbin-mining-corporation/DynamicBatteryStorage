namespace DynamicBatteryStorage
{
  public class ModuleCurvedSolarPanelPowerHandler : ModuleDataHandler
  {
    public ModuleCurvedSolarPanelPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      /// In the case where Kopernicus is installed and using its >= v209 multistar logic, the solar panel management should only work in editor. In flight it is replaced with the Kop model
      /// In the case where WDSP is installed, the solar panel management should only work in editor. In flight it is replaced with the WDSP model
      if ((Settings.Kopernicus && Settings.KopernicusMultiStar) || Settings.WeatherDrivenSolarPanel)
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
