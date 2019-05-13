using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using DynamicBatteryStorage;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{

    public class UIExpandableItem: UIWidget
    {
      private bool visible = false;
      private bool expanded = false;

      private DynamicBatteryStorageUI host;
      private List<ModuleDataHandler> cachedHandlers;
      private List<UICategoryItem> uiItems;

      private string categoryName = "GenericHandledModule";
      private string categoryTotal = "0";
      private string unit = "EC/s";

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="catName">The code name of the category</param>
      /// <param name="catHandlers">The list of handlers for the categorym</param>
      /// <param name="uiHost">The instance of the main UI class</param>
      /// <param name="expandedState">Whether the widget should start extended or not</param>
      public UIExpandableItem(string catName, List<ModuleDataHandler> catHandlers, DynamicBatteryStorageUI uiHost, bool expandedState): base (uiHost)
      {
        host = uiHost;
        expanded = expandedState;
        categoryName = String.Format("{0}", HandlerCategories.HandlerLocalizedNames[catName]);

        RefreshHandlers(catHandlers);

        if (Settings.DebugUIMode)
          Utils.Log(String.Format("[UI]: [UIExpandableItem]: building UI element for category {0}", catName));
      }



      /// <summary>
      /// Draw the UI
      /// </summary>
      public void Draw()
      {
        if (visible)
        {
          GUILayout.BeginVertical(host.GUIResources.GetStyle("block_background"), GUILayout.Width(300f));
          DrawCategoryHeader();
          if (expanded)
            DrawExpanded();
          GUILayout.EndVertical();
        }
      }

      /// <summary>
      /// Draw the expanded list set of all handlers that are part of this field
      /// </summary>
      private void DrawExpanded()
      {
        for (int i=0; i < uiItems.Count; i++)
        {
          DrawExpandedEntry(uiItems[0]);
        }
      }

      /// <summary>
      /// Draw a single expanded entry
      /// </summary>
      /// <param name="handler">The UICategoryItem to draw</param>
      private void DrawExpandedEntry(UICategoryItem handler)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label(handler.PartName, UIHost.GUIResources.GetStyle("data_header"));
        GUILayout.FlexibleSpace();
        GUILayout.Label(handler.PartFlow, UIHost.GUIResources.GetStyle("data_field"));
        GUILayout.EndHorizontal();
      }

      /// <summary>
      /// Draw the category header
      /// </summary>
      private void DrawCategoryHeader()
      {

        if (GUILayout.Button("", UIHost.GUIResources.GetStyle("category_header_button")))
          expanded = !expanded;

        GUI.Label( GUILayoutUtility.GetLastRect(), categoryName, UIHost.GUIResources.GetStyle("category_header"));
        GUI.Label( GUILayoutUtility.GetLastRect(), categoryTotal, UIHost.GUIResources.GetStyle("category_header_field"));

      }

      /// <summary>
      /// Update the UI string fields
      /// </summary>
      public void Update()
      {
        // Get the total flow for the category
        double categoryFlow = 0d;
        for (int i = 0; i < cachedHandlers.Count ; i++)
        {
          categoryFlow += cachedHandlers[i].GetValue();
        }
        categoryTotal = String.Format("{0:F2} {1}", categoryFlow, unit);
        // Update each of the subcategories
        for (int i=0; i < uiItems.Count; i++)
        {
          uiItems[i].Update();
        }
      }

      /// <summary>
      /// Refresh the set of handlers
      /// </summary>
      /// <param name="catHandlers">The list of handlers to supply to this category</param>
      public void RefreshHandlers(List<ModuleDataHandler> catHandlers)
      {
        cachedHandlers = catHandlers;
        uiItems = new List<UICategoryItem>();
        for (int i=0; i < cachedHandlers.Count; i++)
        {
          uiItems.Add(new UICategoryItem(catHandlers[i], unit));
        }
        if (uiItems.Count <= 0)
          visible = false;
        else
          visible = true;
      }
    }

    /// <summary>
    /// Class to store UI data about a single part handler
    /// </summary>
    public class UICategoryItem
    {

      ModuleDataHandler cachedHandler;

      string uiUnit = "EC/s";
      string partName;
      string partFlow;

      public string PartName { get {return partName; }}
      public string PartFlow { get {return partFlow; }}

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="handler">The UICategoryItem to draw</param>
      /// <param name="unit">The units to use</param>
      public UICategoryItem(ModuleDataHandler handler, string unit)
      {
        cachedHandler = handler;

        unit = uiUnit;
        partName = cachedHandler.PartTitle();
        partFlow = String.Format("{0:F2} {1}", cachedHandler.GetValue(), uiUnit);
      }

      /// <summary>
      /// Update the assoicated flow string
      /// </summary>
      public void Update()
      {
        partFlow = String.Format("{0:F2} {1}", cachedHandler.GetValue(), uiUnit);
      }
    }

}
