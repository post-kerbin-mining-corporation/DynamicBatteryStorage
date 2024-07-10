using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicBatteryStorage
{
  /// <summary>
  /// This Vessel Module calculates and stores the Electrical and Core Heat data for a vessel
  /// </summary>
  public class VesselDataManager : VesselModule
  {
    #region Accessors

    public VesselElectricalData ElectricalData { get { return electricalData; } }
    public VesselThermalData HeatData { get { return heatData; } }

    public bool Ready { get { return dataReady; } }

    #endregion

    #region PrivateVariables

    bool dataReady = false;
    bool vesselLoaded = false;

    VesselElectricalData electricalData;
    VesselThermalData heatData;
    #endregion

    protected override void OnStart()
    {
      base.OnStart();

      // These events need to trigger a refresh
      GameEvents.onVesselGoOnRails.Add(new EventData<Vessel>.OnEvent(RefreshVesselData));
      GameEvents.onVesselWasModified.Add(new EventData<Vessel>.OnEvent(RefreshVesselData));
    }

    void OnDestroy()
    {
      // Clean up events when the item is destroyed
      GameEvents.onVesselGoOnRails.Remove(RefreshVesselData);
      GameEvents.onVesselWasModified.Remove(RefreshVesselData);
    }

    void FixedUpdate()
    {
      if (HighLogic.LoadedSceneIsFlight && !dataReady)
      {
        if (!vesselLoaded && FlightGlobals.ActiveVessel == vessel)
        {
          RefreshVesselData();
          vesselLoaded = true;
        }
        if (vesselLoaded && FlightGlobals.ActiveVessel != vessel)
        {
          vesselLoaded = false;
        }
      }
    }

    /// <summary>
    /// Referesh the data, given a Vessel event
    /// </summary>
    protected void RefreshVesselData(Vessel eventVessel)
    {
      if (Settings.DebugMode)
        Utils.Log(String.Format("[{0}]: Refreshing VesselData from Vessel event", this.GetType().Name));
      RefreshVesselData();
    }
    /// <summary>
    /// Referesh the data, given a ConfigNode event
    /// </summary>
    protected void RefreshVesselData(ConfigNode node)
    {
      if (Settings.DebugMode)
        Utils.Log(String.Format("[{0}]: Refreshing VesselData from save node event", this.GetType().Name));
      RefreshVesselData();
    }

    /// <summary>
    /// Referesh the data classes
    /// </summary>
    protected void RefreshVesselData()
    {
      if (vessel == null || vessel.Parts == null)
        return;

      electricalData = new VesselElectricalData(vessel.Parts);
      heatData = new VesselThermalData(vessel.Parts);

      dataReady = true;

      if (Settings.DebugMode)
      {
        Utils.Log(String.Format("Dumping electrical database: \n{0}", electricalData.ToString()));
        Utils.Log(String.Format("Dumping thermal database: \n{0}", heatData.ToString()));
      }
    }
  }
}
