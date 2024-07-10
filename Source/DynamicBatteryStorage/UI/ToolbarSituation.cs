using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
  public class ToolbarSituation
  {
    public CelestialBody SimSituationBody
    {
      get { return currentBody; }
    }
    public float SimSituationAltitude
    {
      get { return altitudeSlider.value * 1000f; }
    }
    public float SimSituationSolarAltitude
    {
      get { return solarAltitudeSlider.value; }
    }
    protected GameObject situationPanel;
    protected RectTransform situationData;

    protected Text situationTitle;
    protected Text bodyTitle;
    protected Dropdown bodyDopdown;

    protected Text altitudeTitle;
    protected Slider altitudeSlider;
    protected Text altitudeLabel;
    protected InputField altitudeTextArea;

    protected Text solarAltitudeTitle;
    protected Slider solarAltitudeSlider;
    protected Text solarAltitudeLabel;
    protected InputField solarAltitudeTextArea;

    protected GameObject situationDataObj;
    protected GameObject situationHeaderObj;

    protected CelestialBody currentBody;

    public void Initialize(Transform root)
    {

      situationHeaderObj = root.FindDeepChild("SituationHeader").gameObject;
      situationDataObj = root.FindDeepChild("SituationData").gameObject;
      situationTitle = root.FindDeepChild("SituationHeaderText").GetComponent<Text>();
      bodyTitle = root.FindDeepChild("BodyLabel").GetComponent<Text>();
      bodyDopdown = root.FindDeepChild("BodyDropdown").GetComponent<Dropdown>();

      altitudeTitle = root.FindDeepChild("AltLabel").GetComponent<Text>();
      altitudeSlider = root.FindDeepChild("AltSlider").GetComponent<Slider>();
      altitudeLabel = root.FindDeepChild("AltUnits").GetComponent<Text>();
      altitudeTextArea = root.FindDeepChild("AltInput").GetComponent<InputField>();

      solarAltitudeTitle = root.FindDeepChild("VelLabel").GetComponent<Text>();
      solarAltitudeSlider = root.FindDeepChild("VelSlider").GetComponent<Slider>();
      solarAltitudeLabel = root.FindDeepChild("VelUnits").GetComponent<Text>();
      solarAltitudeTextArea = root.FindDeepChild("VelInput").GetComponent<InputField>();


      situationData = situationDataObj.GetComponent<RectTransform>();

      if (HighLogic.LoadedSceneIsEditor)
      {
        currentBody = FlightGlobals.GetHomeBody();
        bodyDopdown.AddOptions(FlightGlobals.Bodies.Select(x => x.name).ToList());
        for (int i = 0; i < bodyDopdown.options.Count; i++)
        {
          if (bodyDopdown.options[i].text == currentBody.name)
            bodyDopdown.SetValueWithoutNotify(i);
        }

        SetBody(currentBody);

        bodyDopdown.onValueChanged.AddListener(delegate { OnBodyDropdownChange(); });

        solarAltitudeSlider.onValueChanged.AddListener(delegate { OnSolarAltSliderChange(); });
        solarAltitudeTextArea.onValueChanged.AddListener(delegate { OnSolarAltInputChange(); });

        altitudeSlider.onValueChanged.AddListener(delegate { OnAltSliderChange(); });
        altitudeTextArea.onValueChanged.AddListener(delegate { OnAltInputChange(); });
        SetVisible(true);
      }
      else
      {
        SetVisible(false);
      }
    }
    protected void Localize()
    {
      situationTitle.text = Localizer.Format("#LOC_SystemHeat_ToolbarPanel_SituationTitle");
      bodyTitle.text = Localizer.Format("#LOC_SystemHeat_ToolbarPanel_SituationBody");
      altitudeTitle.text = Localizer.Format("#LOC_SystemHeat_ToolbarPanel_AltitudeTitle");
      altitudeLabel.text = Localizer.Format("#LOC_SystemHeat_ToolbarPanel_AltitudeUnits");
      solarAltitudeTitle.text = Localizer.Format("#LOC_SystemHeat_ToolbarPanel_VelocityTitle");
      solarAltitudeLabel.text = Localizer.Format("#LOC_SystemHeat_ToolbarPanel_VelocityUnits");
    }


    void SetBody(CelestialBody b)
    {

      altitudeSlider.maxValue = (float)b.atmosphereDepth / 1000f;
      altitudeSlider.minValue = 0;
      altitudeSlider.SetValueWithoutNotify((float)b.atmosphereDepth / 1000f);
      altitudeTextArea.SetTextWithoutNotify(altitudeSlider.value.ToString("F0"));

      solarAltitudeSlider.maxValue = b.GetObtVelocity().magnitude;
      solarAltitudeSlider.minValue = 0f;
      solarAltitudeSlider.SetValueWithoutNotify(0f);
      solarAltitudeTextArea.SetTextWithoutNotify("0");

    }
    public void SetVisible(bool visibility)
    {

      situationHeaderObj.SetActive(visibility);
      situationDataObj.SetActive(visibility);
    }


    public void OnBodyDropdownChange()
    {
      Utils.Log($"[ToolbarPanel]: Selected body {bodyDopdown.options[bodyDopdown.value].text}", Utils.LogType.UI);
      foreach (CelestialBody body in FlightGlobals.Bodies)
      {
        if (body.name == bodyDopdown.options[bodyDopdown.value].text)
        {
          currentBody = body;
          SetBody(currentBody);
        }
      }
    }
    public void OnSolarAltSliderChange()
    {
      solarAltitudeTextArea.SetTextWithoutNotify(solarAltitudeSlider.value.ToString("F0"));
    }
    public void OnAltSliderChange()
    {
      altitudeTextArea.SetTextWithoutNotify(altitudeSlider.value.ToString("F0"));
    }
    public void OnSolarAltInputChange()
    {
      solarAltitudeSlider.SetValueWithoutNotify(float.Parse(solarAltitudeTextArea.text));
    }

    public void OnAltInputChange()
    {
      altitudeSlider.SetValueWithoutNotify(float.Parse(altitudeTextArea.text));
    }

    public void OnSeaLevelButtonClicked()
    {
      altitudeSlider.value = 0f;
    }
    public void OnVacButtonClicked()
    {
      altitudeSlider.value = (float)currentBody.atmosphereDepth / 1000f;
    }
    public void OnAltitudeButtonClicked()
    { }
  }
}

