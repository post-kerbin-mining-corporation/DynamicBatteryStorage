using UnityEngine;
using UnityEngine.UI;
using KSP.UI.Screens;

namespace DynamicBatteryStorage.UI
{

  public class ToolbarIconTag
  {
    public enum TagMode
    {
      Critical,
      Alert,
      Warning,
      None
    }
    protected Image alertToolbarIcon;
    protected Image alertToolbarGlow;
    protected Image alertToolbarBackground;
    protected RectTransform alertToolbarRect;
    protected RectTransform alertToolbarGlowRect;
    protected RectTransform alertToolbarBackgroundRect;

    protected Color alertOrange;
    protected Color alertRed;

    protected SystemsMonitorUI mainPanel;
    protected TagMode mode;

    Color currentColor;
    float animateRate = 2f;
    int animateDirection = 1;

    public void Position(ApplicationLauncherButton button)
    {

      alertToolbarBackgroundRect.SetParent(button.toggleButton.transform, false);
      alertToolbarBackgroundRect.anchorMin = Vector2.zero;
      alertToolbarBackgroundRect.anchorMax = Vector3.one;
      alertToolbarBackgroundRect.pivot = Vector2.zero;
      alertToolbarBackgroundRect.offsetMin = new Vector2(22, 24);
      alertToolbarBackgroundRect.offsetMax = new Vector2(0, 3);

      alertToolbarGlowRect.SetParent(alertToolbarBackgroundRect.transform, false);
      alertToolbarGlowRect.anchorMin = Vector2.zero;
      alertToolbarGlowRect.anchorMax = Vector3.one;
      alertToolbarGlowRect.pivot = Vector2.one * 0.5f;
      alertToolbarGlowRect.offsetMin = new Vector2(-5, -5);
      alertToolbarGlowRect.offsetMax = new Vector2(5, 5);

      alertToolbarRect.SetParent(alertToolbarBackgroundRect.transform, false);
      alertToolbarRect.anchorMin = Vector2.zero;
      alertToolbarRect.anchorMax = Vector3.one;
      alertToolbarRect.pivot = Vector2.zero;
      alertToolbarRect.offsetMin = new Vector2(2, 2);
      alertToolbarRect.offsetMax = new Vector2(0, 0);
    }
    public void Initialize(SystemsMonitorUI panel)
    {
      mainPanel = panel;
      GameObject bgObj = new GameObject("SystemsMonitorAlertToolbarBackground");
      GameObject iconObj = new GameObject("SystemsMonitorAlertToolbarIcon");
      GameObject glowObj = new GameObject("SystemsMonitorAlertToolbarGlow");
      alertToolbarIcon = iconObj.AddComponent<Image>();
      alertToolbarBackground = bgObj.AddComponent<Image>();
      alertToolbarGlow = glowObj.AddComponent<Image>();

      alertToolbarBackgroundRect = bgObj.GetComponent<RectTransform>();
      alertToolbarRect = iconObj.GetComponent<RectTransform>();
      alertToolbarGlowRect = glowObj.GetComponent<RectTransform>();

      alertToolbarBackground.color = new Color(0.67f, 0.12f, 0.0039f, 0.9f);
      alertToolbarIcon.sprite = SystemsMonitorAssets.Sprites["monitor-info"];
      alertToolbarGlow.sprite = SystemsMonitorAssets.Sprites["monitor-glow"];
      alertToolbarGlow.color = new Color(0.996f, 0.083f, 0.0039f, 0.0f);

      alertToolbarIcon.enabled = false;
      alertToolbarGlow.enabled = false;
      alertToolbarBackground.enabled = false;
    }

    public void Update(VesselElectricalData data)
    {
      if (data != null)
      {


        data.GetElectricalChargeLevels(out double EC, out double maxEC);
        if (maxEC != 0)
        {
          if (data.CurrentProduction + data.CurrentConsumption < 0.0d)
          {
            if (EC / maxEC <= Settings.UIIconCriticalLevel)
            {
              SetDecreasingPowerCritical();
            }
            else if (EC / maxEC <= Settings.UIIconAlertLevel)
            {
              SetDecreasingPowerAlert();
            }
            else
            {
              SetDecreasingPowerWarning();
            }
          }
          else
          {
            if (EC / maxEC <= 0.0001)
            {
              SetDecreasingPowerCritical();
            }
            else
            {
              SetNoAlert();
            }
          }
        }
        else
        {
          SetNoAlert();

        }

      }
      AnimateGlow();
    }
    protected void SetDecreasingPowerWarning()
    {
      if (!alertToolbarIcon.enabled)
      {
        alertToolbarIcon.enabled = true;
        alertToolbarBackground.enabled = true;
      }
      if (alertToolbarGlow.enabled)
      {
        alertToolbarGlow.enabled = false;
      }
      if (mode != TagMode.Warning)
      {
        alertToolbarIcon.color = new Color(1f, .95f, .65f, 1f);
        alertToolbarBackground.color = new Color(0.67f, 0.523f, 0.0f, 0.65f);
        alertToolbarGlow.color = new Color(0.996f, 0.66f, 0.0039f, 0.0f);
        mode = TagMode.Warning;
      }

    }
    protected void SetDecreasingPowerAlert()
    {
      if (!alertToolbarIcon.enabled)
      {
        alertToolbarIcon.enabled = true;
        alertToolbarBackground.enabled = true;
      }
      if (!alertToolbarGlow.enabled)
      {
        alertToolbarGlow.enabled = true;
      }
      if (mode != TagMode.Alert)
      {
        alertToolbarIcon.color = new Color(1f, .95f, .65f, 1f);
        alertToolbarBackground.color = new Color(0.67f, 0.523f, 0.0f, 0.65f);
        alertToolbarGlow.color = new Color(0.996f, 0.66f, 0.0039f, 0.0f);
        mode = TagMode.Alert;
      }
    }
    protected void SetDecreasingPowerCritical()
    {
      if (!alertToolbarIcon.enabled)
      {
        alertToolbarIcon.enabled = true;
        alertToolbarBackground.enabled = true;
      }
      if (!alertToolbarGlow.enabled)
      {
        alertToolbarGlow.enabled = true;
      }
      if (mode != TagMode.Critical)
      {
        alertToolbarIcon.color = new Color(0.86f, .65f, .48f, 1f);
        alertToolbarBackground.color = new Color(0.67f, 0.12f, 0.0039f, 0.65f);
        alertToolbarGlow.color = new Color(0.996f, 0.083f, 0.0039f, 0.0f);
        mode = TagMode.Critical;
      }
    }
    protected void SetNoAlert()
    {
      if (alertToolbarIcon.enabled)
      {
        alertToolbarIcon.enabled = false;
        alertToolbarGlow.enabled = false;
        alertToolbarBackground.enabled = false;
      }
      mode = TagMode.None;
    }

    protected void AnimateGlow()
    {
      if (alertToolbarGlow != null)
      {
        currentColor = alertToolbarGlow.color;
        currentColor.a += animateRate * animateDirection * Time.deltaTime;
        if (currentColor.a <= 0.0f)
          animateDirection = 1;
        if (currentColor.a >= 1.0f)
          animateDirection = -1;
        alertToolbarGlow.color = currentColor;
      }
    }

  }
}
