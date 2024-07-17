using UnityEngine;
using UnityEngine.UI;
using KSP.UI.Screens;

namespace DynamicBatteryStorage.UI
{
  public class ToolbarIconTag
  {
    public Image alertToolbarIcon;
    public Image alertToolbarGlow;
    public Image alertToolbarBackground;
    public RectTransform alertToolbarRect;
    public RectTransform alertToolbarGlowRect;
    public RectTransform alertToolbarBackgroundRect;


    public ToolbarIconTag() { }
    public void Position(ApplicationLauncherButton button)
    {

      alertToolbarBackgroundRect.SetParent(button.toggleButton.transform, false);
      alertToolbarBackgroundRect.anchorMin = Vector2.zero;
      alertToolbarBackgroundRect.anchorMax = Vector3.one;
      alertToolbarBackgroundRect.pivot = Vector2.zero;
      alertToolbarBackgroundRect.offsetMin = new Vector2(22, 20);
      alertToolbarBackgroundRect.offsetMax = new Vector2(0, 0);

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
      alertToolbarRect.offsetMin = new Vector2(0, 0);
      alertToolbarRect.offsetMax = new Vector2(0, 0);
    }
    public void Initialize()
    {
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
      //alertToolbarIcon.sprite = SystemsMonitorAssets.Sprites["icon_info"];
      //alertToolbarGlow.sprite = SystemsMonitorAssets.Sprites["icon_glow"];
      alertToolbarGlow.color = new Color(0.996f, 0.083f, 0.0039f, 0.0f);
    }
  }
}
