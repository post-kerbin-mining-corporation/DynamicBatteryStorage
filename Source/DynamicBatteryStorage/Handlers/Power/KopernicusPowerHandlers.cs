using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

    // Special power handler for Kopernicus's replaced panels
    public class KopernicusSolarPanelHandler: PowerHandler
    {
      ModuleDeployableSolarPanel panel;

      public override void Initialize(PartModule pm)
      {
          base.Initialize(pm);
        panel = (ModuleDeployableSolarPanel)pm;
      }

      public override double GetPower()
      {
        if (panel != null)
          return (double)panel.flowRate;
        return 0d;
      }
    }
    
}
