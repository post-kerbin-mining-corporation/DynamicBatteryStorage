using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicBatteryStorage
{
  /// <summary>
  /// This  class holds a model of the vessel's thermal data
  /// </summary>
  public class VesselThermalData: VesselData
  {

    /// <param name="vesselParts">Th elist of parts comprising a vessel</param>
    public VesselThermalData(List<Part> vesselParts) : base(vesselParts)
    {}

    /// <summary>
    /// Set up the appropriate HeatHandler component for a PartModule which polls the underlying PartModule for relevant properties
    /// </summary>

    protected override void SetupDataHandler(PartModule pm)
    {
      if (Settings.IsSupportedPartModule(pm.moduleName, ResourcesSupported.Heat))
      {
        HandlerModuleData data = Settings.GetPartModuleData(pm.moduleName, ResourcesSupported.Heat);
        if (Settings.DebugMode)
        {
          Utils.Log(String.Format("[{0}]: Detected supported heat handler for {1}: {2}",  this.GetType().Name, pm.moduleName, data.handlerModuleName));
        }
        ModuleDataHandler handler = (ModuleDataHandler) System.Activator.CreateInstance("DynamicBatteryStorage", "DynamicBatteryStorage." + data.handlerModuleName).Unwrap();
        if( handler.Initialize(pm, data))
          handlers.Add(handler);
      }
    }


    /// <summary>
    /// Dumps the entire handler array as a set of single-line strings defining the handlers on the vessel
    /// </summary>
    public override string ToString()
    {
      List<string> handlerStates = new List<string>();
      if (handlers != null)
      {
        for (int i=0; i < handlers.Count; i++)
        {
          handlerStates.Add(handlers[i].ToString());
        }
        return string.Join("\n -", handlerStates.ToArray());
      }
      return "No Heat Handlers";
    }
  }
}
