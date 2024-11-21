namespace DynamicBatteryStorage
{
  /// <summary>
  /// ModuleAlternator
  /// </summary>
  public class ModuleScienceConverterPowerHandler : ModuleDataHandler
  {
    private ModuleScienceConverter lab;
    public ModuleScienceConverterPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      lab = (ModuleScienceConverter)pm;
      if (lab == null)
        return false;
      return true;
    }

    protected override double GetValueEditor()
    {
      if (lab != null)
      {

        return -1.0d * lab.powerRequirement;
      }
      return 0d;
    }

    protected override double GetValueFlight()
    {
      if (lab != null)
      {
        if (lab.ModuleIsActive())
        {
          return -1.0d * lab.powerRequirement;
        }
      }
      return 0d;
    }
    public override string PartTitle()
    {
      return string.Format("{0} (Science Lab)", base.PartTitle());
    }
  }
}
