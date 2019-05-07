using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{

  /// <summary>
  /// Mode the window is in
  /// </summary>
  enum WindowMode
  {
      Thermal, Electrical
  }

  [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
  public class DynamicBatteryStorageUI : MonoBehaviour
  {
    // Vessel-related variables
    private Vessel activeVessel;
    private VesselDataManager vesselData;
    private EditorVesselDataManager editorVesselData;
    private int partCount = 0;

    static bool showWindow = false;
    static WindowMode windowMode = WindowMode.Electrical;

    // GUI VARS
    private int mainWindowID = new System.Random(3256231).Next();
    private Rect windowPos = new Rect(200f, 200f, 735f, 100f);
    private Vector2 scrollPosition = Vector2.zero;
    private float scrollHeight = 0f;

    private int iconID;
    private static string textVariable;

    bool initUI = false;
    private UIResources resources;
    private UIElectricalView electricalView;
    private UIThermalView thermalView;

    // Stock toolbar button
    private static ApplicationLauncherButton stockToolbarButton = null;

    public UIResources GUIResources { get { return resources; } }

    public VesselThermalData ThermalData { get { return vesselData.ThermalData; } }
    public VesselElectricalData ElectricalData { get { return vesselData.ElectricalData; } }

    /// <summary>
    /// Turn the window on or off
    /// </summary>
    public static void ToggleWindow()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: Toggle Window");
      showWindow = !showWindow;
    }

    /// <summary>
    /// Find the vessel data manager
    /// </summary>
    public void FindData()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        activeVessel = FlightGlobals.ActiveVessel;
        vesselData = activeVessel.GetComponents<VesselDataManager>();
        if (Settings.DebugUIMode)
          Debug.Log("[UI]: Located Flight data");
      }
      if (HighLogic.LoadedSceneIsEditor)
      {
        editorVesselData = EditorBVesselDataManager.Instance;
        if (Settings.DebugUIMode)
          Debug.Log("[UI]: Located Editor data");
      }
    }

    /// <summary>
    /// Initialize the UI comoponents, do localization, set up styles
    /// </summary>
    private void InitUI()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: Initializing");
      resources = new UIResources();
      thermalView = new UIThermalView(this);
      electricalView = new UIElectricalView(this);
      initUI = true;
    }

    private void Awake()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: Awake fired");
      GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
      GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);
    }

    private void Start()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: Start fired");
      if (ApplicationLauncher.Ready)
        OnGUIAppLauncherReady();

      if (HighLogic.LoadedSceneIsFlight)
      {
        FindData();
      }
      if (HighLogic.LoadedSceneIsEditor)
      {

      }
    }

    private void OnGUI()
    {
      if (Event.current.type == EventType.Repaint || Event.current.isMouse) {}
        DrawUI();
    }

    /// <summary>
    /// Draw the UI
    /// </summary>
    private void DrawUI()
    {
      if (!initUI)
        InitUI();

      if (HighLogic.LoadedSceneIsFlight)
      {
        Vessel activeVessel = FlightGlobals.ActiveVessel;
        if (activeVessel != null)
        {
          if (vesselData == null)
            FindData();
        }
      }
      if (HighLogic.LoadedSceneIsEditor)
      {

        if (editorVesselData == null)
          FindData();
      }

      if (showWindow)
      {
        GUI.skin = HighLogic.Skin;
        //windowPos.height = Mathf.Min(scrollHeight + 50f, 96f * 3f + 50f);
        windowPos = GUI.Window(mainWindowID, windowPos, DrawMainWindow, new GUIContent(), GUIResources.GetStyle("window_main"));
      }
    }

    private string windowTitle = "";
    private string windowVersion = "";

    int modeFlag = 0;
    string[] modeStrings = new string[] { "ELECTRICAL", "THERMAL"};

    /// <summary>
    /// Draw the header area
    /// </summary>
    private void DrawHeaderArea()
    {
      GUILayout.BeginHorizontal();
      modeFlag = GUILayout.SelectionGrid(modeFlag, modeStrings, 2, GUIResources.GetStyle("roster_button"));

      GUILayout.FlexibleSpace();
      GUILayout.Label("DBS Window Refactor"), GUIResources.GetStyle("header_basic"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f), GUILayout.MinWidth(350f));

      GUILayout.FlexibleSpace();
      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Rebuild the set of data handlers that will be drawn
    /// </summary>
    /// <param name="windowId">window ID</param>
    private void DrawMainWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawHeaderArea();

      if (vesselData != null)
      {
        GUILayout.Space(3f);
        switch (modeFlag)
        {
          case 0:
            electricalView.Draw();
            break;
          case 1:
            thermalView.Draw();
            break;
        }
      }
      else
      {
        GUILayout.Label("SOMETHING HAS GONE WRONG AAAAAAH");
      }
      GUI.DragWindow();
    }

    void Update()
    {

              // Perform updates of the views
      if (showWindow && (vesselData != null && activeVessel != null) || editorVesselData != null )
      {
        switch (modeFlag)
        {
          case 0:
            electricalView.Update();
            break;
          case 1:
            thermalView.Update();
            break;
        }
      }

      if (HighLogic.LoadedSceneIsFlight)
      {
        // Handle refresh when switching ships
        if (FlightGlobals.ActiveVessel != null)
        {
          if (activeVessel != null)
          {
            if (partCount != activeVessel.parts.Count || activeVessel != FlightGlobals.ActiveVessel)
            {
                ResetAppLauncher();
            }
          }
          else
          {
            ResetAppLauncher();
          }
        }
        if (activeVessel != null)
        {
          if (partCount != activeVessel.parts.Count || activeVessel != FlightGlobals.ActiveVessel)
          {
            ResetAppLauncher();
          }
        }
      }
    }


    void ResetAppLauncher()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: Reset App Launcher");
      FindData();
      if (stockToolbarButton == null)
      {
        stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(
            OnToolbarButtonToggle,
            OnToolbarButtonToggle,
            DummyVoid,
            DummyVoid,
            DummyVoid,
            DummyVoid,
            ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT,
            (Texture)GameDatabase.Instance.GetTexture("NearFutureElectrical/UI/reactor_toolbar_off", false));
      }
      else
      {
      }
    }

    // Stock toolbar handling methods
    public void OnDestroy()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: OnDestroy Fired");
      // Remove the stock toolbar button
      GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
      if (stockToolbarButton != null)
      {
        ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
      }
    }

    private void OnToolbarButtonToggle()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: Toolbar Button Toggled");
      ToggleWindow();
      stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture(showReactorWindow ? "NearFutureElectrical/UI/reactor_toolbar_on" : "NearFutureElectrical/UI/reactor_toolbar_off", false));
    }


    void OnGUIAppLauncherReady()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: App Launcher Ready");
      if (ApplicationLauncher.Ready && stockToolbarButton == null)
      {
        stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(
            OnToolbarButtonToggle,
            OnToolbarButtonToggle,
            DummyVoid,
            DummyVoid,
            DummyVoid,
            DummyVoid,
            ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT,
            (Texture)GameDatabase.Instance.GetTexture("NearFutureElectrical/UI/reactor_toolbar_off", false));
      }
    }

    void OnGUIAppLauncherDestroyed()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: App Launcher Destroyed");
      if (stockToolbarButton != null)
      {
        ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
        stockToolbarButton = null;
      }
    }

    void onAppLaunchToggleOff()
    {
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: App Launcher Toggle Off");
      stockToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture("NearFutureElectrical/UI/reactor_toolbar_off", false));
    }

    void DummyVoid() { }
  }


}
