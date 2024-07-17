using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;
namespace DynamicBatteryStorage.UI
{
  public class ToolbarDetailPart: MonoBehaviour
  {
    public bool PartVisible = false;

    private Toggle simToggle;
    private Text partName;
    private Text partValue;
    private string powerFlowUnits = "";

    private ModuleDataHandler handler;
    public void Awake()
    {
      FindComponents();
    }

    void FindComponents()
    {
      // Find all the components
      simToggle = Utils.FindChildOfType<Toggle>("PartStateToggle", transform);
      partName = Utils.FindChildOfType<Text>("PartDataName", transform);
      partValue = Utils.FindChildOfType<Text>("PartDataValue", transform);

      simToggle.onValueChanged.RemoveAllListeners();
      simToggle.onValueChanged.AddListener((delegate { ToggleSim(); }));

      powerFlowUnits = Localizer.Format("#LOC_DynamicBatteryStorage_UI_ElectricalFlowUnits");
    }

    public void SetPart(ModuleDataHandler newHandler)
    {
      if (partName == null)
      {
        FindComponents();
      }
      handler = newHandler;
      partName.text = newHandler.PartTitle();
      simToggle.SetIsOnWithoutNotify(handler.Simulated);
    }
    
    private void ToggleSim()
    {
      handler.Simulated = simToggle.isOn;
    }

    public void Update()
    {
      if (PartVisible)
      {
        partValue.text = String.Format("{0:F2} {1}", handler.GetValue(), powerFlowUnits);

        if (simToggle.isOn != handler.Simulated)
        {
          simToggle.SetIsOnWithoutNotify(handler.Simulated);
        }
      }
    }

  }
}
