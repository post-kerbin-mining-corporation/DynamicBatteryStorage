using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;

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
  public class ToolbarDetailPanel
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
      detailPanelScrollRect.scrollSensitivity = 20f;
    }

    public void Update(VesselElectricalData electricalData, VesselThermalData thermalData)
    {
      UpdateHandlerList(electricalData, thermalData);
    }

    protected void UpdateHandlerList(VesselElectricalData electricalData, VesselThermalData thermalData)
    {
      // If no cached list, rebuild it from scratch
      if (cachedHandlers == null)
        RebuildCachedList(GetHandlersForUIMode(electricalData, thermalData));

      // If the list changed, rebuild it from components
      var firstNotSecond = GetHandlersForUIMode(electricalData, thermalData).Except(cachedHandlers).ToList();
      var secondNotFirst = cachedHandlers.Except(GetHandlersForUIMode(electricalData, thermalData)).ToList();

      if (firstNotSecond.Any() || secondNotFirst.Any())
      {
        if (Settings.DebugUI)
        {
          Utils.Log("[ToolbarDetailPanel]: Cached handler list does not appear to match the current handler list", Utils.LogType.UI);
        }
        RebuildCachedList(GetHandlersForUIMode(electricalData, thermalData));
      }
    }
    protected List<ModuleDataHandler> GetHandlersForUIMode(VesselElectricalData electricalData, VesselThermalData thermalData)
    {

      if (mode == DetailPanelMode.ConsumerPower)
        return electricalData.VesselConsumers;
      if (mode == DetailPanelMode.ProducerPower)
        return electricalData.VesselProducers;
      if (mode == DetailPanelMode.ConsumerThermal)
        return thermalData.VesselConsumers;
      if (mode == DetailPanelMode.ProducerThermal)
        return thermalData.VesselProducers;

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
            Utils.Log(String.Format("[UI]: [UIView]: Added {0} (Producer = {1}, Consumer = {2} ) to category {3}",
              handlers[i].PartTitle(), handlers[i].Producer, handlers[i].Consumer, categoryEntry.Key), Utils.LogType.UI);
          }
        }
      }

      for (int i = 0; i < categoryNames.Count; i++)
      {
        detailPanelCategoryWidgets[categoryNames[i]].RefreshWithNewHandlers(categories[categoryNames[i]], this);
      }

      // cache the list of handlers to detect changes
      cachedHandlers = new List<ModuleDataHandler>(handlers);
    }

    protected void CreateNewCategoryWidget(UIHandlerCategory associatedCategory)
    {
      Utils.Log("[ToolbarDetailPanel]: Generating a new category widget", Utils.LogType.UI);
      GameObject newObj = (GameObject)GameObject.Instantiate(SystemsMonitorAssets.CategoryItemPrefab, Vector3.zero, Quaternion.identity);
      newObj.transform.SetParent(detailPanelScrollRoot.transform, false);

      ToolbarDetailCategory newWidget = newObj.AddComponent<ToolbarDetailCategory>();
      newWidget.SetCategory(associatedCategory);
      newWidget.SetVisible(false);

      detailPanelCategoryWidgets.Add(associatedCategory.name, newWidget);
      RecalculatePanelPositionData();
    }

    private Vector3 storedButtonPosition;
    public void RecalculatePanelPositionData()
    {
      RecalculatePanelPositionData(storedButtonPosition);
    }
    public void RecalculatePanelPositionData(Vector3 buttonPosition)
    {
      storedButtonPosition = buttonPosition;
      LayoutRebuilder.ForceRebuildLayoutImmediate((detailPanelRootRect));

      // Maximum height of the panel
      float mainPanelMaxHeight = 400f;
      // minimum height (e.g. with 1 member)
      float mainPanelMinHeight = 30f;

      // Calculate the button's y-offset from the top. This depends where the button is. It is the TOP of the button
      float buttonYOffsetFromTop;
      if (mode == DetailPanelMode.ConsumerPower || mode == DetailPanelMode.ConsumerThermal)
      {
        buttonYOffsetFromTop = -230f;
      }
      else
      {
        buttonYOffsetFromTop = -207f;
      }
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
      Utils.Log($"{visibleCount} visible widgets, size is {widgetTotalHeight}", Utils.LogType.UI);
      float calculatedButtonMiddleFromBottom = detailPanel.transform.InverseTransformPoint(buttonPosition).y;
      // Utils.Log($"Source data: buttonYOffsetFromTop={buttonYOffsetFromTop }, loopPanelMaxHeight={mainPanelMaxHeight} widgetTotalHeight={widgetTotalHeight}", Utils.LogType.UI);
      //loopPanelWidgets.Count * 68f + vlg.padding.top+vlg.padding.bottom+ 3f+7.5f*(loopPanelWidgets.Count-1);

      detailPanelRootRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mainPanelMaxHeight);
      detailPanelScrollViewportRect.anchorMin = detailPanelScrollBackground.anchorMin = new Vector2(0, 0);
      detailPanelScrollViewportRect.anchorMax = detailPanelScrollBackground.anchorMax = new Vector2(1, 0);


      //detailPanelScrollCarat.anchorMin = new Vector2(0, 1);
      //detailPanelScrollCarat.anchorMax = new Vector2(0, 1);
      //detailPanelScrollCarat.sizeDelta = new Vector2(17, 17);
      detailPanelScrollCarat.anchoredPosition = new Vector2(256, buttonYOffsetFromTop);

      if (widgetTotalHeight < mainPanelMaxHeight)
      {
        //float topY = Mathf.Min(mainPanelMaxHeight + buttonYOffsetFromTop + widgetTotalHeight / 2f, mainPanelMaxHeight);
        float topY = Mathf.Min(calculatedButtonMiddleFromBottom + widgetTotalHeight / 2f, mainPanelMaxHeight);
        Utils.Log($"Setting scroll viewport rect position anchors to {new Vector2(0, topY) }");
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
    public void SetMode(DetailPanelMode newMode, Vector3 buttonWorldPosition)
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
      RecalculatePanelPositionData(buttonWorldPosition);
    }
    public void SetPanelMode(DetailPanelMode newMode, Vector3 buttonWorldPosition)
    {
      Utils.Log($"[DetailPanel] Setting mode from {mode} to {newMode}, {buttonWorldPosition}", Utils.LogType.UI);
      if (newMode != mode)
      {
        SetMode(newMode, buttonWorldPosition);
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
