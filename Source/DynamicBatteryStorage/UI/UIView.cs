using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DynamicBatteryStorage;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
    public class UIView: UIWidget
    {
      protected DynamicBatteryStorageUI dataHost;

      protected List<ModuleDataHandler> cachedHandlers;

      protected List<string> categoryNames;

      protected Dictionary<string, List<ModuleDataHandler>> producerCats;
      protected Dictionary<string, List<ModuleDataHandler>> consumerCats;

      protected bool showDetails = false;
      protected Vector2 scrollPosition = Vector2.zero;
      protected float scrollHeight = 0f;
      protected float maxScrollHeight = 400f;

      protected float col_width = 280f;
      protected Dictionary<string, UIExpandableItem> producerCategoryUIItems;
      protected Dictionary<string, UIExpandableItem> consumerCategoryUIItems;

      #region GUI Strings
      protected string constantHeader = "";
      protected string simulationHeader = "";
      protected string intermittentHeader = "";
      protected string intermittentToggle = "";
      protected string intermittentOn = "";
      protected string intermittentOff = "";

      protected string totalConsumptionHeader = "";
      protected string totalProductionHeader = "";
      protected string totalConsumption = "";
      protected string totalProduction = "";

      protected string powerFlowUnits = "";
      protected string powerUnits = "";
      protected string heatFlowUnits = "";
      protected string timeUnits = "";

      #endregion

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="uiHost">The instance of the main UI class to link to </param>
      public UIView(DynamicBatteryStorageUI uiHost):base(uiHost)
      {
        dataHost = uiHost;
        col_width = (uiHost.WindowPosition.width -20f)/ 2f;

        // Build all of the base data and UI components
        producerCategoryUIItems = new Dictionary<string, UIExpandableItem>();
        consumerCategoryUIItems = new Dictionary<string, UIExpandableItem>();

        consumerCats = new Dictionary<string, List<ModuleDataHandler>>();
        producerCats = new Dictionary<string, List<ModuleDataHandler>>();
        categoryNames = HandlerCategories.HandlerCategoryMap.Keys.ToList();


        foreach (KeyValuePair<string, List<string>> categoryEntry in HandlerCategories.HandlerCategoryMap)
        {
          consumerCats.Add(categoryEntry.Key, new List<ModuleDataHandler>());
          producerCats.Add(categoryEntry.Key, new List<ModuleDataHandler>());
        }
      }

      /// <summary>
      /// Triggers on creation, localizes relevant strings
      /// </summary>
      protected override void Localize()
      {
        base.Localize();
        powerFlowUnits = Localizer.Format("#LOC_DynamicBatteryStorage_UI_ElectricalFlowUnits");
        powerUnits = Localizer.Format("#LOC_DynamicBatteryStorage_UI_ElectricalUnits");
        heatFlowUnits = Localizer.Format("#LOC_DynamicBatteryStorage_UI_ThermalFlowUnits");
        timeUnits = Localizer.Format("#LOC_DynamicBatteryStorage_UI_TimeUnits");

        simulationHeader = Localizer.Format("#LOC_DynamicBatteryStorage_UI_SimulationPanelTitle");
        constantHeader = Localizer.Format("#LOC_DynamicBatteryStorage_UI_ConstantPanelTitle");
        intermittentHeader = Localizer.Format("#LOC_DynamicBatteryStorage_UI_IntermittentPanelTitle");
        intermittentToggle = Localizer.Format("#LOC_DynamicBatteryStorage_UI_IntermittentToggle");
        intermittentOn = Localizer.Format("#LOC_DynamicBatteryStorage_UI_IntermittentOn");
        intermittentOff  = Localizer.Format("#LOC_DynamicBatteryStorage_UI_IntermittentOff");
      }

      /// <summary>
      /// Main UI method, should do all drawing
      /// </summary>
      public void Draw()
      {
        DrawUpperPanel();
        DrawDetailPanel();
        DrawFooterPanel();
      }

      /// <summary>
      /// Draws the upper panel with flxues and battery states
      /// </summary>
      protected virtual void DrawUpperPanel()
      {

      }

      protected virtual void DrawFooterPanel()
      {
        GUILayout.BeginHorizontal(GUILayout.MaxHeight(80f));
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.Label(simulationHeader, UIHost.GUIResources.GetStyle("panel_header_centered"));
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(UIHost.GUIResources.GetStyle("block_background"));
        GUILayout.Label(constantHeader, UIHost.GUIResources.GetStyle("panel_header_centered"));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(intermittentToggle, UIHost.GUIResources.GetStyle("positive_button")))
          ToggleHandlerSimulationStates(true);
        if (GUILayout.Button(intermittentOn, UIHost.GUIResources.GetStyle("positive_button")))
          SetHandlerSimulationStates(true, true);
        if (GUILayout.Button(intermittentOff, UIHost.GUIResources.GetStyle("positive_button")))
          SetHandlerSimulationStates(true, false);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(UIHost.GUIResources.GetStyle("block_background"));
        GUILayout.Label(intermittentHeader, UIHost.GUIResources.GetStyle("panel_header_centered"));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(intermittentToggle, UIHost.GUIResources.GetStyle("positive_button")))
          ToggleHandlerSimulationStates(true);
        if (GUILayout.Button(intermittentOn, UIHost.GUIResources.GetStyle("positive_button")))
          SetHandlerSimulationStates(true, true);
        if (GUILayout.Button(intermittentOff, UIHost.GUIResources.GetStyle("positive_button")))
          SetHandlerSimulationStates(true, false);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        GUILayout.EndHorizontal();

      }

      /// <summary>
      /// Draws the Detail area with info about detailed production and consumption per module
      /// </summary>
      protected virtual void DrawDetailPanel()
      {

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(" ", UIHost.GUIResources.GetStyle("positive_button"), GUILayout.Width(col_width)))
          showDetails = !showDetails;

        Rect overlayRect = GUILayoutUtility.GetLastRect();

        GUI.Label(overlayRect, totalProductionHeader, UIHost.GUIResources.GetStyle("positive_category_header"));
        GUI.Label(overlayRect, totalProduction, UIHost.GUIResources.GetStyle("positive_category_header_field"));

        if (GUILayout.Button(" ", UIHost.GUIResources.GetStyle("negative_button"), GUILayout.Width(col_width)))
          showDetails = !showDetails;

        overlayRect = GUILayoutUtility.GetLastRect();

        GUI.Label(overlayRect, totalConsumptionHeader, UIHost.GUIResources.GetStyle("negative_category_header"));
        GUI.Label(overlayRect, totalConsumption, UIHost.GUIResources.GetStyle("negative_category_header_field"));

        GUILayout.EndHorizontal();

        if (showDetails)
        {
          scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MinWidth(col_width), GUILayout.MinHeight(Mathf.Min(scrollHeight, maxScrollHeight)));

          float producerScrollHeight = 0f;
          float consumerScrollHeight = 0f;

          GUILayout.BeginHorizontal(GUILayout.Width(col_width*2f));
          GUILayout.BeginVertical(GUILayout.Width(col_width));
          GUILayout.Space(1f);

          for (int i = 0 ; i < categoryNames.Count ; i++)
          {
            producerCategoryUIItems[categoryNames[i]].Draw();
            producerScrollHeight += producerCategoryUIItems[categoryNames[i]].GetHeight();
          }
          GUILayout.EndVertical();
          GUILayout.BeginVertical(GUILayout.Width(col_width));
          GUILayout.Space(1f);
          for (int i = 0 ; i < categoryNames.Count ; i++)
          {
            consumerCategoryUIItems[categoryNames[i]].Draw();
            consumerScrollHeight += consumerCategoryUIItems[categoryNames[i]].GetHeight();
          }
          scrollHeight = Mathf.Max(consumerScrollHeight, producerScrollHeight);
          GUILayout.EndVertical();
          GUILayout.EndHorizontal();
          GUILayout.EndScrollView();
        }

      }

      /// <summary>
      /// Updates the data for drawing - strings and handler data caches
      /// </summary>
      public virtual void Update()
      {

      }

      /// <summary>
      /// Updates the header string data
      /// </summary>
      protected virtual void UpdateHeaderPanelData()
      {}

      /// <summary>
      /// Updates the detail panel data - this is mostly rebuilding the handler list
      /// </summary>
      protected virtual void UpdateDetailPanelData()
      {     }

      /// <summary>
      /// Rebuild the set of data handlers that will be drawn
      /// </summary>
      /// <param name="handlers">The list of data handlers to build the draw list from</param>
      protected void RebuildCachedList(List<ModuleDataHandler> handlers)
      {
        if (Settings.DebugUIMode)
          Utils.Log("[UI]: [UIView]: Rebuilding  handler category map");

        // Rebuild the blank dictionary

        foreach (KeyValuePair<string, List<string>> categoryEntry in HandlerCategories.HandlerCategoryMap)
        {
          consumerCats[categoryEntry.Key] = new List<ModuleDataHandler>();
          producerCats[categoryEntry.Key] = new List<ModuleDataHandler>();
        }

        // Sort through all handlers found and add to the appropriate category
        for (int i = 0; i < handlers.Count; i++)
        {
          foreach(KeyValuePair<string, List<string>> categoryEntry in HandlerCategories.HandlerCategoryMap)
          {
            if (categoryEntry.Value.FindAll(module => module == handlers[i].ModuleName()).Count > 0)
            {
              if (handlers[i].Producer)
                producerCats[categoryEntry.Key].Add(handlers[i]);
              if (handlers[i].Consumer)
                consumerCats[categoryEntry.Key].Add(handlers[i]);

              if (Settings.DebugUIMode)
                Utils.Log(String.Format("[UI]: [UIView]: Added {0} (Producer = {1}, Consumer = {2} ) to category {3}",
                  handlers[i].PartTitle(), handlers[i].Producer, handlers[i].Consumer, categoryEntry.Key));
            }
          }
        }

        for (int i = 0; i < categoryNames.Count; i++)
        {
          producerCategoryUIItems[categoryNames[i]].RefreshHandlers(producerCats[categoryNames[i]]);
          consumerCategoryUIItems[categoryNames[i]].RefreshHandlers(consumerCats[categoryNames[i]]);
        }

        // cache the list of handlers to detect changes
        cachedHandlers = new List<ModuleDataHandler>(handlers);
      }
      protected void ToggleHandlerSimulationStates(bool intermittentOnly)
      {
        for (int i = 0 ; i < cachedHandlers.Count() ; i++)
        {
          if (intermittentOnly && !cachedHandlers[i].TimewarpFunctional)
            cachedHandlers[i].Simulated = !cachedHandlers[i].Simulated;
        }
      }
      protected void SetHandlerSimulationStates(bool state, bool intermittentOnly))
      {
        for (int i = 0 ; i < cachedHandlers.Count() ; i++)
        {
          if (intermittentOnly && !cachedHandlers[i].TimewarpFunctional)
            cachedHandlers[i].Simulated = state;
        }
      }

    }
}
