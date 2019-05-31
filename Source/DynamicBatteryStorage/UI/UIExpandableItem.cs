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
    private float colWidth = 150f;
    private DynamicBatteryStorageUI host;
    private List<ModuleDataHandler> cachedHandlers;
    private List<UICategoryItem> uiItems;

    private string categoryName = "GenericHandledModule";
    private string categoryTotal = "0";
    private string uiUnits = "EC/s";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="catName">The code name of the category</param>
    /// <param name="catHandlers">The list of handlers for the categorym</param>
    /// <param name="uiHost">The instance of the main UI class</param>
    /// <param name="expandedState">Whether the widget should start extended or not</param>
    public UIExpandableItem(string catName, List<ModuleDataHandler> catHandlers, DynamicBatteryStorageUI uiHost, bool expandedState, float width, string uiUnit): base (uiHost)
    {
      uiUnits = uiUnit;
      host = uiHost;
      expanded = expandedState;
      categoryName = String.Format("{0}", HandlerCategories.HandlerLocalizedNames[catName]);
      colWidth = width;

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
        GUILayout.BeginVertical(host.GUIResources.GetStyle("block_background"), GUILayout.Width(colWidth));
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
        DrawExpandedEntry(uiItems[i]);
      }
    }

    /// <summary>
    /// Draw a single expanded entry
    /// </summary>
    /// <param name="handler">The UICategoryItem to draw</param>
    private void DrawExpandedEntry(UICategoryItem handler)
    {
      if (handler.PartHandler.IsVisible())
      {
        GUILayout.BeginHorizontal();
        handler.PartHandler.Simulated = GUILayout.Toggle(handler.PartHandler.Simulated, "", UIHost.GUIResources.GetStyle("button_toggle"));
        GUILayout.Label(handler.PartName, UIHost.GUIResources.GetStyle("data_header"));
        GUILayout.FlexibleSpace();
        GUILayout.Label(handler.PartFlow, UIHost.GUIResources.GetStyle("data_field"));
        GUILayout.EndHorizontal();
      }
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
    public void Update(float scalar)
    {
      // Get the total flow for the category
      double categoryFlow = 0d;
      for (int i = 0; i < cachedHandlers.Count ; i++)
      {
        if (cachedHandlers[i].Simulated)
          categoryFlow += cachedHandlers[i].GetValue(scalar);
      }
      categoryTotal = String.Format("{0:F2} {1}", Math.Abs(categoryFlow), uiUnits);
      // Update each of the subcategories
      for (int i=0; i < uiItems.Count; i++)
      {
        uiItems[i].Update(scalar);
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
      int visItems = 0;
      for (int i=0; i < cachedHandlers.Count; i++)
      {
        uiItems.Add(new UICategoryItem(catHandlers[i], uiUnits));
        if (catHandlers[i].IsVisible())
        {
          visItems++;
        }
      }
      if (cachedHandlers.Count > 0 && visItems > 0)
      {
        visible = true;
      }
      else
      {
        visible = false;
      }
    }
  }

  /// <summary>
  /// Class to store UI data about a single part handler
  /// </summary>
  public class UICategoryItem
  {

    ModuleDataHandler cachedHandler;

    string uiUnit = "EC/s";
    string partName = "Part";
    string partFlow = "0.0";

    public string PartName { get {return partName; }}
    public string PartFlow { get {return partFlow; }}
    public ModuleDataHandler PartHandler { get {return cachedHandler; }}

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="handler">The UICategoryItem to draw</param>
    /// <param name="unit">The units to use</param>
    public UICategoryItem(ModuleDataHandler handler, string unit)
    {
      cachedHandler = handler;

      uiUnit = unit;
      partName = cachedHandler.PartTitle();
      partFlow = String.Format("{0:F2} {1}", Math.Abs(cachedHandler.GetValue(1.0f)), uiUnit);
    }

    /// <summary>
    /// Update the assoicated flow string
    /// </summary>
    public void Update()
    {
      Update(1.0f);
    }
    /// <summary>
    /// Update the assoicated flow string
    /// </summary>
    public void Update(float scalar)
    {
      partFlow = String.Format("{0:F2} {1}", Math.Abs(cachedHandler.GetValue(scalar)), uiUnit);
    }
  }

}
