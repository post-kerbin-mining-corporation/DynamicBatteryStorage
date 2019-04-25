using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicBatteryStorage
{
  /// <summary>
  /// This  class holds a custom set of information about a vessel's modules that needs to be rebuilt or monitored
  /// </summary>
  public class VesselData
  {

    Vessel vessel;

    public VesselData(Vessel v)
    {
      vessel = v;
      RefreshData();
    }

    /// <summary>
    /// Referesh the data
    /// </summary>
    public void RefreshData()
    {
      ClearData();
      if (vessel == null || vessel.Parts == null)
      {
        Utils.Warn(String.Format("[{0}]: Refresh failed for vessel, vessel or parts list is null", this.GetType().Name));
        return;
      }
      for (int i = vessel.Parts.Count - 1; i >= 0; --i)
      {
        Part part = vessel.Parts[i];
        for (int j = part.Modules.Count - 1; j >= 0; --j)
        {
          PartModule m = part.Modules[j];
          SetupDataHandler(m);
        }
      }
    }

    /// <summary>
    /// Dumps a string representation of the set of data
    /// </summary>
    public virtual void ToString()
    {
      return "";
      // Implement this
    }

    /// <summary>
    /// Clears the data
    /// </summary>
    protected virtual void ClearData()
    {
      // Implement this to clean up the vessel data modules
    }

    /// <summary>
    /// Sets up a part module's data handler
    /// </summary>
    protected virtual void SetupDataHandler(PartModule m)
    {
      // Implement this
    }
  }

  public class VesselElectricalData: VesselData
  {

    List<PowerHandler> powerHandlers;

    public VesselElectricalData(Vessel v)
    {
      vessel = v;
      powerHandlers = new List<PowerHandler>();
      RefreshData();
    }

    /// <summary>
    /// Clear the data
    /// </summary>
    protected override void ClearData()
    {
      powerHandlers.Clear();
    }

    /// <summary>
    /// Set up the appropriate PowerHandler component for a PartModule which polls the underlying PartModule for relevant properties
    /// The PartModue must be supported as in the enums defined in PowerHandlerType
    /// </summary>
    protected override void SetupDataHandler(PartModule pm)
    {
      PowerHandlerType handlerType;
      if (Enum.TryParse(pm.moduleName, out handlerType))
      {
        string typeName =  "DynamicBatteryStorage."+ pm.moduleName + "Handler";
        if (Settings.DebugMode)
        {
          Utils.Log(String.Format("[{0}]: Detected supported power handler of type: {1}",  this.GetType().Name, Type.GetType(pm.moduleName + "Handler"+",DynamicBatteryStorage")));
        }
        PowerHandler handler = (PowerHandler) System.Activator.CreateInstance("DynamicBatteryStorage", typeName).Unwrap();
        handler.Initialize(pm);
        powerHandlers.Add(handler);
      }
    }

    /// <summary>
    /// Dumps the entire handler array as a set of single-line strings defining the handlers on the vessel
    /// </summary>
    public override void ToString()
    {
      List<string> handlerStates = new List<string>();
      if (powerHandlers)
      {
        for (int i=0; i < powerHandlers.Count; i++)
        {
          handlerStates.Add(powerHandlers[i].ToString());
        }
        return string.Join("\n", handlerStates);
      }
      return "No Power Handlers";
    }

    /// <summary>
    /// Calculates total vessel power consumption
    /// </summary>
    public double CurrentConsumption {
      get {
        double consumption = 0d;
        for (int i=0; i < powerHandlers.Count; i++)
        {
          double pwr = powerHandlers[i].GetPower();
          if (pwr < 0d)
            consumption += pwr;
        }
        return consumption;
      }
    }

    /// <summary>
    /// Calculates total vessel power production
    /// </summary>
    public double CurrentProduction {
      get {
        double production = 0d;
        for (int i=0; i < powerHandlers.Count; i++)
        {
          double pwr = powerHandlers[i].GetPower();
          if (pwr > 0d)
            production += pwr;
        }
        return production;
      }
    }

    /// <summary>
    /// Gets a list of power producing modules on the vessel
    /// </summary>
    public List<PowerHandler> VesselProducers
    {
      get
      {
        List<PowerHandler> handlers = new List<PowerHandler>();
        for (int i=0; i < powerHandlers.Count; i++)
        {
          if (powerHandlers[i].IsProducer())
          {
            handlers.Add(powerHandlers[i]);
          }
        }
        return handlers;
      }
    }

    /// <summary>
    /// Gets a list of power consuming modules on the vessel
    /// </summary>
    public List<PowerHandler> VesselConsumers
    {
      get
      {
        List<PowerHandler> handlers = new List<PowerHandler>();
        for (int i=0; i < powerHandlers.Count; i++)
        {
          if (!powerHandlers[i].IsProducer())
          {
            handlers.Add(powerHandlers[i]);
          }
        }
        return handlers;
      }
    }
  }
}
