using System;

namespace DynamicBatteryStorage
{

  // Special power handler for Kopernicus's replaced panels
  public class KopernicusSolarPanelPowerHandler : ModuleDeployableSolarPanelPowerHandler
  {
   
    public KopernicusSolarPanelPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
     
      return true;
    }
  }
  public class KopernicusCurvedSolarPanelPowerHandler : ModuleCurvedSolarPanelPowerHandler
  {

    public KopernicusCurvedSolarPanelPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      return true;
    }
  }

}
