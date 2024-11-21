using System;

namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleScienceLab
  /// </summary>
  public class ModuleScienceLabPowerHandler : ModuleDataHandler
  {
    ModuleScienceLab lab;
    double processRate = 0d;
    public ModuleScienceLabPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      lab = (ModuleScienceLab)pm;
      return true;
    }

    protected override double GetValueEditor()
    {
      if (lab != null)
      {

        for (int i = 0; i < lab.processResources.Count; i++)
        {
          if (lab.processResources[i].id == PartResourceLibrary.ElectricityHashcode)
          {
            processRate = -1.0d * lab.processResources[i].amount;
            return processRate;
          }
        }
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (lab != null)
      {
        if (lab.IsOperational())
          return processRate;
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return String.Format("{0} (Clean Experiments)", base.PartTitle());
    }
  }

}
