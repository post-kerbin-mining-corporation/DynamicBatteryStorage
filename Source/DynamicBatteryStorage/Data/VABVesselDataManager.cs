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
    public bool Ready { get { return dataReady; } }

    #endregion

    #region PrivateVariables

    bool dataReady = false;

    VesselElectricalData electricalData;
    #endregion

    protected void Awake()
    {
      enabled = Settings.Enabled;
      Instance = this;
      SetupEditorCallbacks();
    }
    #region Editor
    protected void SetupEditorCallbacks()
    {
      GameEvents.onEditorShipModified.Add(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
      GameEvents.onEditorRestart.Add(new EventVoid.OnEvent(onEditorVesselReset));
      GameEvents.onEditorStarted.Add(new EventVoid.OnEvent(onEditorVesselStart));
      GameEvents.onEditorPartDeleted.Add(new EventData<Part>.OnEvent(onEditorPartDeleted));
      GameEvents.onEditorPodDeleted.Add(new EventVoid.OnEvent(onEditorVesselReset));
      GameEvents.onEditorLoad.Add(new EventData<ShipConstruct, KSP.UI.Screens.CraftBrowserDialog.LoadType>.OnEvent(onEditorVesselLoad));
      GameEvents.onPartRemove.Add(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(onEditorVesselPartRemoved));
    }

    void OnDestroy()
    {
      GameEvents.onEditorShipModified.Remove(new EventData<ShipConstruct>.OnEvent(onEditorVesselModified));
      GameEvents.onEditorRestart.Remove(new EventVoid.OnEvent(onEditorVesselReset));
      GameEvents.onEditorStarted.Remove(new EventVoid.OnEvent(onEditorVesselStart));
      GameEvents.onEditorPodDeleted.Remove(new EventVoid.OnEvent(onEditorVesselReset));
      GameEvents.onEditorPartDeleted.Remove(new EventData<Part>.OnEvent(onEditorPartDeleted));
      GameEvents.onEditorLoad.Remove(new EventData<ShipConstruct, KSP.UI.Screens.CraftBrowserDialog.LoadType>.OnEvent(onEditorVesselLoad));
      GameEvents.onPartRemove.Remove(new EventData<GameEvents.HostTargetAction<Part, Part>>.OnEvent(onEditorVesselPartRemoved));
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

        Utils.Log(String.Format("Dumping electrical database: \n{0}", electricalData.ToString()), Utils.LogType.VesselData);
        dataReady = true;
      }
      else
      {
        Utils.Log(String.Format("Ship is null"), Utils.LogType.VesselData);
        electricalData = new VesselElectricalData(new List<Part>());
      }
    }

    protected void RemovePart(Part p)
    {
      electricalData.RemoveHandlersForPart(p);
    }
    #endregion


    #region Game Events
    public void onEditorPartDeleted(Part part)
    {
      Utils.Log("[VAB VesselDataManager][Editor]: Part DELETED", Utils.LogType.VesselData);
      if (!HighLogic.LoadedSceneIsEditor) { return; }

      InitializeEditorConstruct(EditorLogic.fetch.ship, false);
    }
    public void onEditorVesselReset()
    {
      Utils.Log("[VAB VesselDataManager][Editor]: Vessel RESET", Utils.LogType.VesselData);
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(EditorLogic.fetch.ship, true);
    }
    public void onEditorVesselStart()
    {
      Utils.Log("[VAB VesselDataManager][Editor]: Vessel START", Utils.LogType.VesselData);
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(EditorLogic.fetch.ship, true);
    }
    public void onEditorVesselRestore()
    {
      Utils.Log("[VAB VesselDataManager]: Vessel RESTORE", Utils.LogType.VesselData);
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(EditorLogic.fetch.ship, true);
    }
    public void onEditorVesselLoad(ShipConstruct ship, KSP.UI.Screens.CraftBrowserDialog.LoadType type)
    {
      Utils.Log("[VAB VesselDataManager][Editor]: Vessel LOAD", Utils.LogType.VesselData);
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(ship, true);
    }
    public void onEditorVesselPartRemoved(GameEvents.HostTargetAction<Part, Part> p)
    {
      Utils.Log("[VAB VesselDataManager][Editor]: Vessel PART REMOVED", Utils.LogType.VesselData);
      if (!HighLogic.LoadedSceneIsEditor) { return; }

      if (electricalData == null)
        InitializeEditorConstruct(EditorLogic.fetch.ship, false);
      else
        RemovePart(p.target);
    }
    public void onEditorVesselModified(ShipConstruct ship)
    {
      Utils.Log("[VAB VesselDataManager][Editor]: Vessel MODIFIED", Utils.LogType.VesselData);
      if (!HighLogic.LoadedSceneIsEditor) { return; }
      InitializeEditorConstruct(ship, false);
    }
    #endregion
  }

}
