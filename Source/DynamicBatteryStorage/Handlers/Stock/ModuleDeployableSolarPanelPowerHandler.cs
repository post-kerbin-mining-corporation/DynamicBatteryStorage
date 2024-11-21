namespace DynamicBatteryStorage
{
  /// <summary>
  /// Deployable Solar Panel
  /// </summary>
  public class ModuleDeployableSolarPanelPowerHandler : ModuleDataHandler
  {
    ModuleDeployableSolarPanel panel;
    ModuleResource resource;

    double cachedRate = 0d;
    public ModuleDeployableSolarPanelPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      panel = (ModuleDeployableSolarPanel)pm;
      cachedRate = panel.chargeRate;
      if (panel.resHandler.outputResources != null && panel.resHandler.outputResources.Count > 0)
      {
        for (int i = 0; i < panel.resHandler.outputResources.Count; i++)
        {
          if (panel.resHandler.outputResources[i].id == PartResourceLibrary.ElectricityHashcode)
          {
            resource = panel.resHandler.outputResources[i];
          }
        }
      }
      /// If kopernicus is using the multistar settings, we need to show this in the editor but NOT in flight
      if (Settings.Kopernicus && Settings.KopernicusMultiStar || Settings.WeatherDrivenSolarPanel)
      {
        return HighLogic.LoadedSceneIsEditor;
      }
      return true;
    }

    protected override double GetValueEditor()
    {
      double rate = 0d;
      if (panel != null)
      {
        if (resource != null)
        {
          rate = resource.rate;
        }
        else
        {
          rate = panel.chargeRate;
        }

        if (Settings.Kopernicus && Settings.KopernicusMultiStar)
        {
          rate = cachedRate;
        }

        if (panel.panelType == ModuleDeployableSolarPanel.PanelType.SPHERICAL)
        {
          rate *= 0.25d;
        }
        if (panel.panelType == ModuleDeployableSolarPanel.PanelType.CYLINDRICAL)
        {
          rate *= 0.3183d;
        }
      }
      return rate * solarEfficiency;
    }
    protected override double GetValueFlight()
    {
      if (panel != null)
        return panel.flowRate;
      return 0d;
    }
  }

}
