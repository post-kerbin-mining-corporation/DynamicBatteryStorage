namespace DynamicBatteryStorage
{
  /// <summary>
  /// This is a generic handler that parses a KSPField
  /// Ideally this field is populated by another mod then read by this
  /// </summary>
  public class GenericFieldDataHandler : ModuleDataHandler
  {
    private string editorFieldName;
    private string flightFieldName;

    private double editorValueScalar = 1.0d;
    private double flightValueScalar = 1.0d;

    public GenericFieldDataHandler(HandlerModuleData moduleData) : base(moduleData)
    {
      editorFieldName = moduleData.config.editorFieldName;
      flightFieldName = moduleData.config.flightFieldName;
      editorValueScalar = moduleData.config.editorValueScalar;
      flightValueScalar = moduleData.config.flightValueScalar;
    }

    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      return true;
    }
    protected override double GetValueEditor()
    {
      double.TryParse(pm.Fields.GetValue(editorFieldName).ToString(), out double results);
      return results * editorValueScalar;
    }
    protected override double GetValueFlight()
    {
      double.TryParse(pm.Fields.GetValue(flightFieldName).ToString(), out double results);
      return results * flightValueScalar;
    }
  }
}
