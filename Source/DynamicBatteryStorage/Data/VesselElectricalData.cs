using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DynamicBatteryStorage.Power;

namespace DynamicBatteryStorage
{

  /// <summary>
  /// This  class holds a model of the vessel's electrical data
  /// </summary>
  public class VesselElectricalData: VesselData
  {

    /// <param name="vesselParts">Th elist of parts comprising a vessel</param>
    public VesselElectricalData(List<Part> vesselParts) : base(vesselParts)
    {}

    /// <summary>
    /// Set up the appropriate PowerHandler component for a PartModule which polls the underlying PartModule for relevant properties
    /// The PartModue must be supported as in the enums defined in PowerHandlerType
    /// </summary>
    protected override void SetupDataHandler(PartModule pm)
    {
      PowerHandlerType handlerType;
      if (Enum.TryParse(pm.moduleName, out handlerType))
      {
        string typeName =  "DynamicBatteryStorage."+ pm.moduleName + "PowerHandler";
        if (Settings.DebugMode)
        {
          Utils.Log(String.Format("[{0}]: Detected supported power handler of type: {1}",  this.GetType().Name, Type.GetType(pm.moduleName + "PowerHandler"+",DynamicBatteryStorage")));
        }
        ModuleDataHandler handler = (ModuleDataHandler) System.Activator.CreateInstance("DynamicBatteryStorage", typeName).Unwrap();
        handler.Initialize(pm);
        handlers.Add(handler);
      }
    }

    /// <summary>
    /// Dumps the entire handler array as a set of single-line strings defining the handlers on the vessel
    /// </summary>
    public override void ToString()
    {
      List<string> handlerStates = new List<string>();
      if (handlers)
      {
        for (int i=0; i < handlers.Count; i++)
        {
          handlerStates.Add(handlers[i].ToString());
        }
        return string.Join("\n", handlerStates);
      }
      return "No Power Handlers";
    }


  }
}
