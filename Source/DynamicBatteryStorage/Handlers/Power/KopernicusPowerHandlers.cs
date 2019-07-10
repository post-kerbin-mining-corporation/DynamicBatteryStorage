using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

  // Special power handler for Kopernicus's replaced panels
  public class KopernicusSolarPanelPowerHandler: ModuleDataHandler
  {
    ModuleDeployableSolarPanel panel;

    public KopernicusSolarPanelPowerHandler(HandlerModuleData moduleData):base(moduleData)
    {}

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      panel = (ModuleDeployableSolarPanel)pm;
      return true;
    }

    protected override double GetValueEditor()
    {
      if (panel != null)
        return (double)panel.chargeRate * solarEfficiency;
      return 0d;
    }
    protected override double GetValueFlight()
    {
      if (panel != null)
        return (double)panel.flowRate;
      return 0d;
    }
  }

}
