using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI;
using KSP.UI.Screens;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
  public class ToolbarPanel : MonoBehaviour
  {
    public float SimSituationAltitude
    {
      get { return situationUI.SimSituationAltitude; }
    }
    public float SimSituationSolarAltitude
    {
      get { return situationUI.SimSituationSolarAltitude; }
    }
    public CelestialBody SimSituationBody
    {
      get { return situationUI.SimSituationBody; }
    }

    public void SetToolbarPosition(Vector2 newPos)
    {
      rect.position = newPos;
    }
    public ToolbarDetailPanel DetailUI
    {
      get { return detailUI; }
    }

    protected ToolbarSituation situationUI;
    protected ToolbarPower powerUI;
    protected ToolbarDetailPanel detailUI;

    protected bool active = true;
    protected bool panelOpen = false;
    protected RectTransform rect;

    protected Text panelTitle;

    public void Awake()
    {
      // Find all the components
      rect = this.GetComponent<RectTransform>();
      panelTitle = Utils.FindChildOfType<Text>("PanelTitleText", transform);

      // Loop Panel
      detailUI = new ToolbarDetailPanel();
      detailUI.Initialize(transform);

      // Craft Stats
      powerUI = new ToolbarPower();
      powerUI.Initialize(transform, detailUI);

      // Situation
      situationUI = new ToolbarSituation();
      situationUI.Initialize(transform);


      Localize();
    }
    void Localize()
    {
      panelTitle.text = Localizer.Format("#LOC_DynamicBatteryStorage_ToolbarPanel_Title");
    }

    protected void Update()
    {
      if (powerUI != null)
      {
        powerUI.Update();
      }
      if (detailUI != null)
      {
        detailUI.Update();
      }
    }

    public void SetVisible(bool state)
    {
      active = state;
      rect.gameObject.SetActive(state);
    }
  }
}
