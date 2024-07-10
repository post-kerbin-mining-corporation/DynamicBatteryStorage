using System;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicBatteryStorage
{

  /// <summary>
  /// This version of the VesselDataManager runs in the VAB
  /// </summary>
  [KSPAddon(KSPAddon.Startup.EditorAny, false)]
  public class EditorVesselDataManager : MonoBehaviour
  {
    #region Accessors

    public static EditorVesselDataManager Instance { get; private set; }
    public VesselElectricalData ElectricalData { get { return electricalData; } }
    public VesselThermalData HeatData { get { return heatData; } }

    public bool Ready { get { return dataReady; } }

    #endregion

    #region PrivateVariables

    bool dataReady = false;

    VesselElectricalData electricalData;
    VesselThermalData heatData;
    #endregion

    protected void Awake()
    {
      enabled = Settings.Enabled;
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
        GameEvents.onEditorPartDeleted.Add(new EventData<Part>.OnEvent(onEditorPartDeleted));
        GameEvents.onEditorPodDeleted.Add(new EventVoid.OnEvent(onEditorVesselReset));
        GameEvents.onEditorLoad.Add(new EventData<ShipConstruct, KSP.UI.Screens.CraftBrowserDialog.LoadType>.OnEvent(onEditorVesselLoad));
        GameEvents.onPartRemove.Add(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(onEditorVesselPartRemoved));
      }
      else
      {
        GameEvents.onEditorShipModified.Remove(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
        GameEvents.onEditorRestart.Remove(new EventVoid.OnEvent(onEditorVesselReset));
        GameEvents.onEditorStarted.Remove(new EventVoid.OnEvent(onEditorVesselStart));
        GameEvents.onEditorPodDeleted.Remove(new EventVoid.OnEvent(onEditorVesselReset));
        GameEvents.onEditorPartDeleted.Remove(new EventData<Part>.OnEvent(onEditorPartDeleted));
        GameEvents.onEditorLoad.Remove(new EventData<ShipConstruct, KSP.UI.Screens.CraftBrowserDialog.LoadType>.OnEvent(onEditorVesselLoad));
        GameEvents.onPartRemove.Remove(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(onEditorVesselPartRemoved));
      }
    }

    protected void InitializeEditorConstruct(ShipConstruct ship, bool forceReset)
    {
      dataReady = false;
      if (ship != null)
      {
        if (electricalData == null || forceReset)
          electricalData = new VesselElectricalData(ship.Parts);
        else
          electricalData.RefreshData(false, ship.Parts);

        if (heatData == null || forceReset)
          heatData = new VesselThermalData(ship.Parts);
        else
          heatData.RefreshData(false, ship.Parts);

        if (Settings.DebugMode)
        {
          Utils.Log(String.Format("Dumping electrical database: \n{0}", electricalData.ToString()));
          Utils.Log(String.Format("Dumping thermal database: \n{0}", heatData.ToString()));
        }
        dataReady = true;
      }
      else
      {
        if (Settings.DebugMode)
        {
          Utils.Log(String.Format("Ship is null"));
        }
        electricalData = new VesselElectricalData(new List<Part>());
        heatData = new VesselThermalData(new List<Part>());
      }
    }

    protected void RemovePart(Part p)
    {

      electricalData.RemoveHandlersForPart(p);
      heatData.RemoveHandlersForPart(p);

    }
    #endregion


    #region Game Events
    public void onEditorPartDeleted(Part part)
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager][Editor]: Part Delete");
      if (!HighLogic.LoadedSceneIsEditor) { return; }

      InitializeEditorConstruct(EditorLogic.fetch.ship, false);
    }
    public void onEditorVesselReset()
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager][Editor]: Vessel RESET");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(EditorLogic.fetch.ship, true);
    }
    public void onEditorVesselStart()
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager]: Vessel START");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(EditorLogic.fetch.ship, true);
    }
    public void onEditorVesselLoad(ShipConstruct ship, KSP.UI.Screens.CraftBrowserDialog.LoadType type)
    {
      if (Settings.DebugMode)
        Utils.Log("[RadioactivitySimulator][Editor]: Vessel LOAD");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(ship, true);
    }
    public void onEditorVesselPartRemoved(GameEvents.HostTargetAction<Part, Part> p)
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager][Editor]: Vessel PART REMOVE");
      if (!HighLogic.LoadedSceneIsEditor) { return; }

      if (electricalData == null || heatData == null)
        InitializeEditorConstruct(EditorLogic.fetch.ship, false);
      else
        RemovePart(p.target);
    }
    public void onEditorVesselModified(ShipConstruct ship)
    {
      if (Settings.DebugMode)
        Utils.Log("[VAB VesselDataManager][Editor]: Vessel MODIFIED");
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(ship, false);
    }
    #endregion
  }

}
