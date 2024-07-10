namespace DynamicBatteryStorage
{
  /// <summary>
  /// Defines data for a sepecific type of handler
  /// </summary>
  public class HandlerConfiguration
  {
    public string editorFieldName = "";
    public string flightFieldName = "";

    public double editorValueScalar = 1.0d;
    public double flightValueScalar = 1.0d;

    private const string EDITOR_FIELDNAME_PARAMETER_NAME = "editorFieldName";
    private const string FLIGHT_FIELDNAME_PARAMETER_NAME = "flightFieldName";
    private const string EDITOR_SCALAR_PARAMETER_NAME = "editorValueScalar";
    private const string FLIGHT_SCALAR_PARAMETER_NAME = "flightValueScalar";

    public HandlerConfiguration(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue(EDITOR_FIELDNAME_PARAMETER_NAME, ref editorFieldName);
      node.TryGetValue(FLIGHT_FIELDNAME_PARAMETER_NAME, ref flightFieldName);
      node.TryGetValue(FLIGHT_SCALAR_PARAMETER_NAME, ref flightValueScalar);
      node.TryGetValue(EDITOR_SCALAR_PARAMETER_NAME, ref editorValueScalar);

      if (editorFieldName == "" && flightFieldName == "")
      {
        Utils.Log("At least one of editorFieldName or flightFieldName must be specified in HANDLER_CONFIG for GenericFieldDataHandler");
      }
      else if (editorFieldName == "")
      {
        editorFieldName = flightFieldName;
      }
      else if (flightFieldName == "")
      {
        flightFieldName = editorFieldName;
      }
    }

  }
}
