using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicBatteryStorage
{

  /// <summary>
  /// This version of the VesselDataManager runs in the VAB
  /// </summary>
  [KSPAddon(KSPAddon.Startup.Editor, false)]
  public class EditorVesselDataManager: MonoBehaviour
  {
    #region Accessors

    public static EditorVesselDataManager Instance { get; private set; }
    public VesselElectricalData ElectricalData { get {return electricalData; }}
    public VesselThermalData HeatData { get {return heatData; }}

    public bool Ready { get {return dataReady; }}

    #endregion

    #region PrivateVariables

    bool dataReady = false;

    VesselElectricalData electricalData;
    VesselThermalData heatData;
    #endregion

    protected void Awake()
    {
      Instance = this;
    }
    protected void Start()
    {
      SetupEditorCallbacks();
    }

    #region Editor
    protected void SetupEditorCallbacks()
    {
      /// Add events for editor modifications
      if (HighLogic.LoadedSceneIsEditor)
      {
        GameEvents.onEditorShipModified.Add(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
        GameEvents.onEditorRestart.Add(new EventVoid.OnEvent(onEditorVesselReset));
        GameEvents.onEditorStarted.Add(new EventVoid.OnEvent(onEditorVesselStart));
        GameEvents.onEditorLoad.Add(new EventData<ShipConstruct, KSP.UI.Screens.CraftBrowserDialog.LoadType>.OnEvent(onEditorVesselLoad));
        GameEvents.onPartRemove.Add(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(onEditorVesselPartRemoved));
      }
      else
      {
        GameEvents.onEditorShipModified.Remove(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
        GameEvents.onEditorRestart.Remove(new EventVoid.OnEvent(onEditorVesselReset));
        GameEvents.onEditorStarted.Remove(new EventVoid.OnEvent(onEditorVesselStart));
        GameEvents.onEditorLoad.Remove(new EventData<ShipConstruct, KSP.UI.Screens.CraftBrowserDialog.LoadType>.OnEvent(onEditorVesselLoad));
        GameEvents.onPartRemove.Remove(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(onEditorVesselPartRemoved));
      }
    }

    protected void InitializeEditorConstruct(ShipConstruct ship)
    {
      dataReady = false;
      if (ship != null)
      {
        electricalData = new VesselElectricalData(ship.Parts);
        thermalData = new VesselThermalData(ship.Parts);

        if (Settings.DebugMode)
        {
          Utils.Log(String.Format("Dumping electrical database: \n{}", electricalData.ToString() ) );
          Utils.Log(String.Format("Dumping thermal database: \n{}", heatData.ToString() ) );
        }
        dataReady = true;
      }

    }
    #endregion


    #region Game Events
    public void onEditorVesselReset()
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager][Editor]: Vessel RESET");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(EditorLogic.fetch.ship);
    }
    public void onEditorVesselStart()
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager]: Vessel START");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(EditorLogic.fetch.ship);
    }
    public void onEditorVesselLoad(ShipConstruct ship, KSP.UI.Screens.CraftBrowserDialog.LoadType type)
    {
      if (Settings.DebugMode)
        Utils.Log("[RadioactivitySimulator][Editor]: Vessel LOAD");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(ship);
    }
    public void onEditorVesselPartRemoved(GameEvents.HostTargetAction<Part, Part> p)
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager][Editor]: Vessel PART REMOVE");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(EditorLogic.fetch.ship);
    }
    public void onEditorVesselModified(ShipConstruct ship)
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager][Editor]: Vessel MODIFIED");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(ship);
    }
    #endregion
  }

}
