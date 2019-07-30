using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{
  // This is a generic handler that parses a KSPField
  public class GenericFieldDataHandler: ModuleDataHandler
  {
    string editorFieldName;
    string flightFieldName;

    double editorValueScalar = 1.0d;
    double flightValueScalar = 1.0d;

    public GenericFieldDataHandler(HandlerModuleData moduleData):base(moduleData)
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
      double results = 0d;
      double.TryParse(pm.Fields.GetValue(editorFieldName).ToString(), out results);
      return results* editorValueScalar;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue(flightFieldName).ToString(), out results);
      return results* flightValueScalar;
    }
  }
}
