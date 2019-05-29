using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
  class UISolarPanelManager:UIWidget
  {
    UIElectricalView electricalView;

    // Variables for body simulation
    CelestialBody sunBody;
    CelestialBody homeBody;
    CelestialBody selectedBody;
    int selectedBodyIndex;
    string selectedBodyName;
    // height above sun in km
    double sunRefOrbitHeight = 0d;
    // height above selected body in km
    double bodyRefOrbitHeight = 0d;
    // predicted efficiency of solar panels
    float panelScalar = 1f;
    double occlusionTime = 0d;
    // Home body orbit altitude
    double refSunOrbitAlt = 0d;

    // UI strings
    string panelName = "";
    string planetName = "";
    string planetNameTitle = "";
    string solarAltitudeTitle = "";
    string solarAltitude = "";
    string bodyAltitudeTitle = "";
    string bodyAltitude = "";
    string panelEfficiencyTitle = "";
    string panelEfficiency = "";
    string darkTimeTitle = "";
    string darkTime = "";
    string distanceUnits = "";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="uiHost">The parent UI</param>
    /// <param name="view">The parent ElectricalView panel</param>
    public UISolarPanelManager(DynamicBatteryStorageUI uiHost, UIElectricalView view): base (uiHost)
    {
      electricalView = view;

      sunBody = FlightGlobals.Bodies[0];
      homeBody = FlightGlobals.GetHomeBody();

      selectedBodyIndex = homeBody.flightGlobalsIndex;
      SelectBody(homeBody);

      refSunOrbitAlt = FlightGlobals.getAltitudeAtPos(homeBody.getPositionAtUT(0d), sunBody)/1000000d;

      if (Settings.DebugUIMode)
        Utils.Log(String.Format("[UI Solar Manager] Set home body to {0} (alt {1}), sun body to {2}", homeBody.name, FormatUtils.ToSI(refSunOrbitAlt, "F2"), sunBody.name));

      if (Settings.DebugUIMode)
        Utils.Log(String.Format("[UI Solar Manager] Created"));
    }

    /// <summary>
    /// Do localization of UI strings
    /// </summary>
    protected override void Localize()
    {
      base.Localize();
      panelName = Localizer.Format("LOC_DynamicBatteryStorage_UI_SolarManagerTitle");
      solarAltitudeTitle = Localizer.Format("LOC_DynamicBatteryStorage_UI_SolarAltitudeTitle");
      bodyAltitudeTitle = Localizer.Format("LOC_DynamicBatteryStorage_UI_OrbitAltitudeTitle");
      panelEfficiencyTitle = Localizer.Format("LOC_DynamicBatteryStorage_UI_SolarEfficiencyTitle");
      darkTimeTitle = Localizer.Format("LOC_DynamicBatteryStorage_UI_EclipseTimeTitle");
      planetNameTitle = Localizer.Format("LOC_DynamicBatteryStorage_UI_BodySelectionTitle");
      distanceUnits = Localizer.Format("LOC_DynamicBatteryStorage_UI_DistanceUnits");
    }

    /// <summary>
    /// Draw method
    /// </summary>
    public void Draw()
    {
      GUILayout.BeginVertical(UIHost.GUIResources.GetStyle("block_background"));

      GUILayout.Label(panelName, UIHost.GUIResources.GetStyle("panel_header_centered"));
      // Select planet

      GUILayout.BeginHorizontal();
      GUILayout.Label(planetNameTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      if (GUILayout.Button(planetName, UIHost.GUIResources.GetStyle("radio_text_button"), GUILayout.MaxWidth(100f)))
        IncrementBody();
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label(solarAltitudeTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      sunRefOrbitHeight = (double)GUILayout.HorizontalSlider((float)sunRefOrbitHeight, 5f, 50000f, GUILayout.MaxWidth(120f));
      GUILayout.Label(solarAltitude, UIHost.GUIResources.GetStyle("data_field"), GUILayout.MinWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label(bodyAltitudeTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      bodyRefOrbitHeight = (double)GUILayout.HorizontalSlider((float)bodyRefOrbitHeight, 1f, 50000000, GUILayout.MaxWidth(120f));
      GUILayout.Label(bodyAltitude, UIHost.GUIResources.GetStyle("data_field"), GUILayout.MinWidth(60f));
      GUILayout.EndHorizontal();


      GUILayout.BeginHorizontal();
      GUILayout.Label(panelEfficiencyTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      GUILayout.Label(panelEfficiency, UIHost.GUIResources.GetStyle("data_field"));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label(darkTimeTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      GUILayout.Label(darkTime, UIHost.GUIResources.GetStyle("data_field"));
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
    }

    /// <summary>
    /// Increment the selected CelestialBody
    /// </summary>
    private void IncrementBody()
    {
      double prevOrbitalHeight = sunRefOrbitHeight;

      if (selectedBodyIndex >= FlightGlobals.Bodies.Count-1)
        selectedBodyIndex = 0;
      else
        selectedBodyIndex++;

      SelectBody(FlightGlobals.Bodies[selectedBodyIndex]);

    }

    /// <summary>
    /// Update UI fields and recalculate quantities
    /// </summary>>
    public void Update()
    {
      solarAltitude = String.Format("{0}{1}", FormatUtils.ToSI(sunRefOrbitHeight * 1000, "F0"), distanceUnits);
      bodyAltitude = String.Format("{0}{1}", FormatUtils.ToSI(bodyRefOrbitHeight * 1000, "F0"), distanceUnits);
      panelEfficiency = String.Format("{0:F1}%", panelScalar*100f);
      darkTime = FormatUtils.FormatTimeString(occlusionTime);
      // Determine scaling constant
      panelScalar = CalculatePanelScalar();
      occlusionTime = CalculateOcclusionTime();
      electricalView.SolarSimulationScalar = panelScalar;
    }

    /// <summary>
    /// Select a specific CelestialBody
    /// </summary>
    /// <param name="body">The CelestialBody to select</param>
    protected void SelectBody(CelestialBody body)
    {
      selectedBody = body;

      planetName = selectedBody.name;
      if (selectedBody == sunBody)
      { }
      else
      {
        sunRefOrbitHeight = FlightGlobals.getAltitudeAtPos(selectedBody.getPositionAtUT(0d), sunBody)/1000000d;
      }
      bodyRefOrbitHeight = (selectedBody.atmosphereDepth + 10000d)/1000d;

      if (Settings.DebugUIMode)
        Utils.Log(String.Format("[UI Solar Manager] Selected {0}", selectedBody.name));
    }

    /// <summary>
    /// Calculates a simulated eclipse time for orbit around a specific body
    /// </summary>
    protected double CalculateOcclusionTime()
    {
      if (selectedBodyIndex == 0)
        return 0d;

        // This is a gemetric approximation assuming a circular equatorial orbit and a cylindrical solar occlusion
      // Note the scaling factors for KM here
      double scaling = 1000.0d;
      double r = selectedBody.Radius / scaling;
      double h = (r + bodyRefOrbitHeight);
      double orb_vel = Math.Sqrt(selectedBody.gravParameter/Math.Pow(scaling, 3)/h);
      return Math.Asin(r / h)* h ) / orb_vel;
    }

    /// <summary>
    /// Calculates the scaling factor for solar panel power at this solar altitude
    /// </summary>
    protected float CalculatePanelScalar()
    {
      return (float)((1d / sunRefOrbitHeight) *(refSunOrbitAlt) ) ;
    }

  }
}
