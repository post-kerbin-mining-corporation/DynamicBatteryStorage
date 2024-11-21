﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;
using System.Collections;

namespace DynamicBatteryStorage.UI
{
  public enum DetailPanelMode
  {
    ConsumerPower,
    ConsumerThermal,
    ProducerPower,
    ProducerThermal,
    None
  }
  public class ToolbarDetailPanel: MonoBehaviour
  {
    public GameObject detailPanel;

    public GameObject detailPanelScrollRoot;
    public RectTransform detailPanelScrollBackground;
    public RectTransform detailPanelScrollCarat;
    public RectTransform detailPanelScrollRootRect;
    public RectTransform detailPanelScrollViewportRect;

    protected ScrollRect detailPanelScrollRect;
    protected RectTransform detailPanelRootRect;
    protected RectTransform basePanelRootRect;

    protected GameObject noDataObject;
    protected Text noDataText;

    protected Dictionary<string, ToolbarDetailCategory> detailPanelCategoryWidgets;

    protected bool shown = false;
    private RectTransform storedButtonRect;
    protected DetailPanelMode mode = DetailPanelMode.None;

    protected Dictionary<string, List<ModuleDataHandler>> categories;
    protected List<ModuleDataHandler> cachedHandlers;
    protected List<string> categoryNames;

    public void Initialize(Transform root)
    {
      cachedHandlers = new List<ModuleDataHandler>();

      detailPanelCategoryWidgets = new Dictionary<string, ToolbarDetailCategory>();

      detailPanel = root.FindDeepChild("PanelCategories").gameObject;
      detailPanelRootRect = detailPanel.GetComponent<RectTransform>();
      detailPanelScrollRect = detailPanel.GetComponent<ScrollRect>();

      basePanelRootRect = root.FindDeepChild("PanelBase").GetComponent<RectTransform>();

      detailPanelScrollRoot = root.FindDeepChild("Scrolly").gameObject;
      detailPanelScrollRootRect = detailPanelScrollRoot.GetComponent<RectTransform>();
      detailPanelScrollViewportRect = root.FindDeepChild("ScrollViewPort").GetComponent<RectTransform>();

      detailPanelScrollCarat = root.FindDeepChild("CategoryCarat").GetComponent<RectTransform>();
      detailPanelScrollBackground = root.FindDeepChild("CategoryBackground").GetComponent<RectTransform>();

      noDataText = Utils.FindChildOfType<Text>("NoDataText", root);
      noDataObject = root.FindDeepChild("NoData").gameObject;

      categoryNames = Settings.HandlerCategories;
      categories = new Dictionary<string, List<ModuleDataHandler>>();
      foreach (KeyValuePair<string, UIHandlerCategory> categoryEntry in Settings.HandlerCategoryData)
      {
        categories.Add(categoryEntry.Key, new List<ModuleDataHandler>());
        CreateNewCategoryWidget(categoryEntry.Value);
      }
      SetVisible(shown);
      RecalculatePanelPositionData();
      detailPanelScrollRect.scrollSensitivity = 20f;
    }

    public void UpdateData(VesselElectricalData electricalData)
    {
      UpdateHandlerList(electricalData);
    }

    protected void UpdateHandlerList(VesselElectricalData electricalData)
    {
      // If no cached list, rebuild it from scratch
      if (cachedHandlers == null)
        RebuildCachedList(GetHandlersForUIMode(electricalData));

      // If the list changed, rebuild it from components
      var firstNotSecond = GetHandlersForUIMode(electricalData).Except(cachedHandlers).ToList();
      var secondNotFirst = cachedHandlers.Except(GetHandlersForUIMode(electricalData)).ToList();

      if (firstNotSecond.Any() || secondNotFirst.Any())
      {
        if (Settings.DebugUI)
        {
          Utils.Log("[ToolbarDetailPanel]: Cached handler list does not appear to match the current handler list", Utils.LogType.UI);
        }
        RebuildCachedList(GetHandlersForUIMode(electricalData));
      }
    }
    protected List<ModuleDataHandler> GetHandlersForUIMode(VesselElectricalData electricalData)
    {

      if (mode == DetailPanelMode.ConsumerPower)
        return electricalData.VesselConsumers;
      if (mode == DetailPanelMode.ProducerPower)
        return electricalData.VesselProducers;

      return electricalData.VesselConsumers;
    }
    protected void RebuildCachedList(List<ModuleDataHandler> handlers)
    {
      Utils.Log("[ToolbarDetailPanel]: Rebuilding handler category map", Utils.LogType.UI);

      // Rebuild the blank dictionary
      foreach (KeyValuePair<string, UIHandlerCategory> categoryEntry in Settings.HandlerCategoryData)
      {
        categories[categoryEntry.Key] = new List<ModuleDataHandler>();
      }

      // Sort through all handlers found and add to the appropriate category
      for (int i = 0; i < handlers.Count; i++)
      {
        foreach (KeyValuePair<string, UIHandlerCategory> categoryEntry in Settings.HandlerCategoryData)
        {
          if (categoryEntry.Value.handledModules.FindAll(module => module == handlers[i].ModuleName()).Count > 0)
          {

            categories[categoryEntry.Key].Add(handlers[i]);
            Utils.Log(String.Format("[ToolbarDetailPanel]: [UIView]: Added {0} (Producer = {1}, Consumer = {2} ) to category {3}",
              handlers[i].PartTitle(), handlers[i].Producer, handlers[i].Consumer, categoryEntry.Key), Utils.LogType.UI);
          }
        }
      }
      for (int i = 0; i < categoryNames.Count; i++)
      {
        detailPanelCategoryWidgets[categoryNames[i]].RefreshWithNewHandlers(categories[categoryNames[i]], this);
      }
      RecalculatePanelPositionData();
      // cache the list of handlers to detect changes
      cachedHandlers = new List<ModuleDataHandler>(handlers);
    }

    protected void CreateNewCategoryWidget(UIHandlerCategory associatedCategory)
    {
      Utils.Log($"[ToolbarDetailPanel]: Generating a new category widget for {associatedCategory.title}", Utils.LogType.UI);
      GameObject newObj = (GameObject)GameObject.Instantiate(SystemsMonitorAssets.CategoryItemPrefab, Vector3.zero, Quaternion.identity);
      newObj.transform.SetParent(detailPanelScrollRoot.transform, false);

      ToolbarDetailCategory newWidget = newObj.AddComponent<ToolbarDetailCategory>();
      newWidget.SetCategory(associatedCategory);
      newWidget.SetVisible(false);

      detailPanelCategoryWidgets.Add(associatedCategory.name, newWidget);
    }
    public void RecalculatePanelPositionData()
    {
      StartCoroutine(RecalculatePanelPositionData(storedButtonRect));
    }

    public IEnumerator RecalculatePanelPositionData(RectTransform buttonRect)
    {
      storedButtonRect = buttonRect;
      LayoutRebuilder.ForceRebuildLayoutImmediate((detailPanelRootRect));
      yield return 0f;
      // Maximum height of the panel
      float mainPanelMaxHeight = 400f;
      // minimum height (e.g. with 1 member)
      float mainPanelMinHeight = 30f;

      // Calculate the total height of the widgets in the categories
      float widgetTotalHeight = 0f;
      int visibleCount = 0;
      foreach (KeyValuePair<string, ToolbarDetailCategory> kvp in detailPanelCategoryWidgets)
      {
        if (kvp.Value.Visible)
        {
          widgetTotalHeight += kvp.Value.GetHeight() + 4f;
          visibleCount++;
        }
      }
      if (visibleCount == 0)
      {
        widgetTotalHeight = mainPanelMinHeight;
        noDataObject.SetActive(true);
      }
      else
      {
        noDataObject.SetActive(false);
      }
      Utils.Log($"[ToolbarDetailPanel] {visibleCount} visible widgets, size is {widgetTotalHeight}", Utils.LogType.UI);
      if (buttonRect == null)
      {
        Utils.Log($"[ToolbarDetailPanel] button rect is not set up yet", Utils.LogType.UI);
        yield break;
      }
      float calculatedButtonMiddleFromBottom = detailPanel.transform.InverseTransformPoint(storedButtonRect.position).y;
     // Utils.Log($"[ToolbarDetailPanel] Source data: buttonYOffsetFromTop={calculatedButtonMiddleFromBottom }, loopPanelMaxHeight={mainPanelMaxHeight} widgetTotalHeight={widgetTotalHeight}", Utils.LogType.UI);

      detailPanelRootRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainPanelMaxHeight);
      detailPanelScrollViewportRect.anchorMin = detailPanelScrollBackground.anchorMin = detailPanelScrollCarat.anchorMin = new Vector2(0, 0);
      detailPanelScrollViewportRect.anchorMax = detailPanelScrollBackground.anchorMax = detailPanelScrollCarat.anchorMax = new Vector2(1, 0);

      detailPanelScrollCarat.anchoredPosition = new Vector2(0, calculatedButtonMiddleFromBottom + 9f);

      if (widgetTotalHeight < mainPanelMaxHeight)
      {
        //float topY = Mathf.Min(mainPanelMaxHeight + buttonYOffsetFromTop + widgetTotalHeight / 2f, mainPanelMaxHeight);
        float topY = Mathf.Min(calculatedButtonMiddleFromBottom + widgetTotalHeight / 2f, mainPanelMaxHeight);
        Utils.Log($"[ToolbarDetailPanel] Setting scroll viewport rect position anchors to {new Vector2(0, topY) }", Utils.LogType.UI);
        /// there will be no scrolling, set the position of the scroll rect to the top and have fun
        detailPanelScrollViewportRect.anchoredPosition = new Vector2(0, topY);
        detailPanelScrollViewportRect.sizeDelta = new Vector2(4f, widgetTotalHeight);

        detailPanelScrollBackground.anchoredPosition = new Vector2(0, topY);
        detailPanelScrollBackground.sizeDelta = new Vector2(4.5f, widgetTotalHeight);
      }
      else
      {
        /// set height to max
        detailPanelScrollViewportRect.anchoredPosition = new Vector2(0, mainPanelMaxHeight);
        detailPanelScrollViewportRect.sizeDelta = new Vector2(4f, mainPanelMaxHeight);
        detailPanelScrollBackground.anchoredPosition = new Vector2(0, mainPanelMaxHeight);
        detailPanelScrollBackground.sizeDelta = new Vector2(4.5f, mainPanelMaxHeight);
      }
    }

    public void SetVisible(bool visibility)
    {
      detailPanel.SetActive(visibility);
    }
    public void SetMode(DetailPanelMode newMode, RectTransform buttonRect)
    {
      mode = newMode;
      if (mode == DetailPanelMode.ConsumerPower || mode == DetailPanelMode.ConsumerThermal)
      {
        noDataText.text = Localizer.Format("#LOC_DynamicBatteryStorage_DetailPanel_NoConsumers");
      }
      else
      {
        noDataText.text = Localizer.Format("#LOC_DynamicBatteryStorage_DetailPanel_NoGenerators");
      }
      StartCoroutine(RecalculatePanelPositionData(buttonRect));
    }
    public void SetPanelMode(DetailPanelMode newMode, RectTransform buttonRect)
    {
      Utils.Log($"[ToolbarDetailPanel] Setting mode from {mode} to {newMode}, {buttonRect.position}", Utils.LogType.UI);
      if (newMode != mode)
      {
        SetMode(newMode, buttonRect);
        if (!shown)
        {
          shown = true;
        }
      }
      else
      {
        shown = !shown;
      }
      SetVisible(shown);

    }
  }
}
