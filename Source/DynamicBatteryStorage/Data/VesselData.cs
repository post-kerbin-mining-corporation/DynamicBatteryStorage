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

    protected List<Part> parts;
    protected List<PartModule> modules;
    protected List<ModuleDataHandler> handlers;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="vesselParts">Th elist of parts comprising a vessel</param>
    public VesselData(List<Part> vesselParts)
    {
      handlers = new List<ModuleDataHandler>();
      modules = new List<PartModule>();
      RefreshData(true, vesselParts);
    }

    /// <summary>
    /// Refresh the data
    /// </summary>
    public void RefreshData(bool fromScratch, List<Part> vesselParts)
    {
      parts = vesselParts;
      if (fromScratch)
      {
        ClearData();
      }

      if (parts == null)
      {
        Utils.Warn(String.Format("[{0}]: Refresh failed for vessel, vessel or parts list is null", this.GetType().Name));
        return;
      }
      List<PartModule> newModules = new List<PartModule>();

      for (int i = parts.Count - 1; i >= 0; --i)
      {
        Part part = parts[i];
        for (int j = part.Modules.Count - 1; j >= 0; --j)
        {
          newModules.Add(part.Modules[j]);
        }
      }

      List<PartModule> modulesToSetup = newModules.Except(modules).ToList();

      for (int i = handlers.Count - 1; i >= 0; --i)
      {
        if (handlers[i] == null || handlers[i].PM == null || handlers[i].PM.part == null || !handlers[i].PM.part.isAttached)
        {
          handlers.RemoveAt(i);
          if (Settings.DebugMode)
          {
            Utils.Log(String.Format("A handler was removed"));
          }
        }
      }
      if (Settings.DebugMode)
      {
        Utils.Log(String.Format("[{0}]: Checking {1} new candidate PMs", this.GetType().Name, modulesToSetup.Count));
      }
      for (int i = modulesToSetup.Count - 1; i >= 0; --i)
      {
        SetupDataHandler(modulesToSetup[i]);
        modules.Add(modulesToSetup[i]);
      }
    }

    /// <summary>
    /// Removes handlers associated with a specific part
    /// </summary>
    /// <param name="p">The part whose handlers to strip.</param>
    public void RemoveHandlersForPart(Part p)
    {

      for (int i = handlers.Count - 1; i >= 0; --i)
      {
        if (handlers[i].PM.part == p)
        {
          for (int j = modules.Count - 1; j >= 0; --j)
          {
            if (modules[j] == handlers[i].PM)
            {
              modules.RemoveAt(j);
              if (Settings.DebugMode)
              {
                Utils.Log(String.Format("A module was removed"));
              }
            }
          }
          handlers.RemoveAt(i);
          if (Settings.DebugMode)
          {
            Utils.Log(String.Format("A modulehandler was removed"));
          }
        }
      }
    }
    /// <summary>
    /// Dumps a string representation of the set of data
    /// </summary>
    public override string ToString()
    {
      return "";
      // Implement this
    }

    /// <summary>
    /// Clears the data
    /// </summary>
    protected virtual void ClearData()
    {
      handlers.Clear();
      modules.Clear();
    }

    /// <summary>
    /// Sets up a part module's data handler
    /// </summary>
    /// <param name="m">The PartModule to build the data handler off of.</param>
    protected virtual void SetupDataHandler(PartModule m)
    {
      // Implement this
    }

    /// <summary>
    /// Remove a part module's data handler
    /// </summary>
    /// <param name="m">The PartModule to build the data handler off of.</param>
    protected virtual void RemoveDataHandler(PartModule m)
    {
      for (int i = handlers.Count - 1; i >= 0; --i)
      {
        if (handlers[i].PM == null)
        {
          handlers.RemoveAt(i);
        }
      }
    }

    public List<ModuleDataHandler> AllHandlers { get { return handlers; } }

    /// <summary>
    /// Calculates total vessel consumption
    /// </summary>
    public double CurrentConsumption
    {
      get
      {
        double consumption = 0d;
        for (int i = 0; i < handlers.Count; i++)
        {
          if (HighLogic.LoadedSceneIsFlight || handlers[i].Simulated)
          {
            double pwr = handlers[i].GetValue();
            if (pwr < 0d)
              consumption += pwr;
          }
        }
        return consumption;
      }
    }

    /// <summary>
    /// Calculates total vessel  production
    /// </summary>
    public double CurrentProduction
    {
      get
      {
        double production = 0d;
        for (int i = 0; i < handlers.Count; i++)
        {
          if (HighLogic.LoadedSceneIsFlight || handlers[i].Simulated)
          {
            double pwr = handlers[i].GetValue();
            if (pwr > 0d)
              production += pwr;
          }
        }
        return production;
      }
    }

    /// <summary>
    /// Gets a list of producing modules on the vessel
    /// </summary>
    public List<ModuleDataHandler> VesselProducers
    {
      get
      {
        return handlers.FindAll(handler => handler.Producer);
      }
    }

    /// <summary>
    /// Gets a list of consuming modules on the vessel
    /// </summary>
    public List<ModuleDataHandler> VesselConsumers
    {
      get
      {
        return handlers.FindAll(handler => handler.Consumer);
      }
    }

    /// <summary>
    /// Gets a list of modules on the vessel of a specific type
    /// </summary>
    /// <param name="moduleName">The module name to find</param>
    /// <param name="producers">Whether to get producers or consumers</param>
    public List<ModuleDataHandler> GetVesselModulesByType(string moduleName, bool producers)
    {
      List<ModuleDataHandler> candidates;
      if (producers)
        candidates = VesselProducers;
      else
        candidates = VesselConsumers;
      return candidates.FindAll(handler => handler.ModuleName() == "moduleName");
    }
  }
}
