using System;
using System.Collections.Generic;

namespace DynamicBatteryStorage
{

  /// <summary>
  /// This  class holds a model of the vessel's electrical data
  /// </summary>
  public class VesselElectricalData : VesselData
  {

    /// <param name="vesselParts">Th elist of parts comprising a vessel</param>
    public VesselElectricalData(List<Part> vesselParts) : base(vesselParts)
    { }

    /// <summary>
    /// Set up the appropriate PowerHandler component for a PartModule which polls the underlying PartModule for relevant properties
    /// </summary>
    protected override void SetupDataHandler(PartModule pm)
    {
      if (Settings.IsSupportedPartModule(pm.moduleName, ResourcesSupported.Power))
      {
        HandlerModuleData data = Settings.GetPartModuleData(pm.moduleName, ResourcesSupported.Power);
        Utils.Log(String.Format("[{0}]: Detected supported power handler for {1}: {2}", this.GetType().Name, pm.moduleName, data.handlerModuleName), Utils.LogType.VesselData);


        string typeName = this.GetType().AssemblyQualifiedName;
        typeName = typeName.Replace("VesselElectricalData", data.handlerModuleName);
        try
        {
          ModuleDataHandler handler = (ModuleDataHandler)System.Activator.CreateInstance(Type.GetType(typeName), data);
          if (handler.Initialize(pm))
            handlers.Add(handler);
        }
        catch (ArgumentNullException)
        {
          Utils.Log(String.Format("Failed to instantiate {0} (config as {1}) when trying to configure a handler for {2}", typeName, data.handlerModuleName, pm.moduleName), Utils.LogType.VesselData);
        }
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
        for (int i = 0; i < handlers.Count; i++)
        {
          handlerStates.Add(handlers[i].ToString());
        }
        return string.Join("\n -", handlerStates.ToArray());
      }
      return "No Power Handlers";
    }

    public void GetElectricalChargeLevels(out double EC, out double maxEC)
    {

      if (HighLogic.LoadedSceneIsEditor)
      {
        EditorLogic.fetch.ship.UpdateResourceSets();
        EditorLogic.fetch.ship.GetConnectedResourceTotals(PartResourceLibrary.ElectricityHashcode, true, out EC, out maxEC, true);
      }
      else if (HighLogic.LoadedSceneIsFlight)
      {
        FlightGlobals.ActiveVessel.GetConnectedResourceTotals(PartResourceLibrary.ElectricityHashcode, out EC, out maxEC);
      }
      else
      {
        EC = 0d;
        maxEC = 0d;
      }
    }

    public double GetSimulatedElectricalProdution()
    {

      if (HighLogic.LoadedSceneIsEditor)
      {

        double production = 0d;
        for (int i = 0; i < handlers.Count; i++)
        {
          if (handlers[i].Simulated)
          {
            double pwr = handlers[i].GetValue();
            if (pwr > 0d)
              production += pwr;
          }
        }
        return production;
      }
      else
      {
        return CurrentProduction;
      }
    }
  }
}
