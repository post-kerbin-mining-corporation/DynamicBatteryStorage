using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{

  public class GenericFieldDataHandler: ModuleDataHandler
  {

    string editorFieldName;
    string flightFieldName;

    public GenericFieldDataHandler(HandlerModuleData moduleData):base(moduleData)
    {
      solarEfficiencyEffects = false;
      visible = true;
      simulated = true;
      timewarpFunctional = true;
      producer = false;
      consumer = true;
    }
    public override bool Initialize(PartModule pm)
    {
      base.Initialize(pm);
      return false;
    }
    protected override double GetValueEditor()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue(editorFieldName).ToString(), out results);
      return results* -1.0d;
    }
    protected override double GetValueFlight()
    {
      double results = 0d;
      double.TryParse(pm.Fields.GetValue(flightFieldName).ToString(), out results);
      return results* -1.0d;
    }
  }
}
