using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicBatteryStorage
{

  /// <summary>
  /// This Vessel Module calculates vessel energy production and consumption, and chooses a part to act as a 'battery buffer' at high time warp speeds
  ///
  /// </summary>
  public class ModuleDynamicBatteryStorage : VesselModule
  {
    public bool AnalyticMode {get {return analyticMode;}}

    public Part BufferPart { get { return bufferPart; } }
    public PartResource BufferResource { get { return bufferStorage; } }

    public double BufferScale { get { return bufferScale; } }
    public double BufferSize { get { return bufferSize; } }
    public double MaxEC { get { return originalMax + bufferDifference; } }

    public double SavedMaxEC { get { return originalMax; } }
    public double SavedVesselMaxEC { get { return totalEcMax; } }

    #region PrivateVariables

    float timeWarpLimit = 100f;
    double bufferScale = 1.5d;

    bool vesselLoaded = false;
    bool analyticMode = false;
    bool dataReady = false;
    bool hasBuffer = false;
    double bufferSize = 0d;

    // The base, non-buffered EC total of the buffer part
    double originalMax = 0d;
    // The base, non-buffered EC total of the Vessel
    double totalEcMax = 0d;
    // The amount to be added to originalMax to get the needed buffer size
    double bufferDifference = 0d;

    VesselDataManager vesselData;
    PartResource bufferStorage;
    Part bufferPart;

    #endregion

    protected override void  OnStart()
    {
      base.OnStart();

      bufferScale = (double)Settings.BufferScaling;
      timeWarpLimit = Settings.TimeWarpLimit;

      GameEvents.onVesselDestroy.Add(new EventData<Vessel>.OnEvent(CalculateElectricalData));
      GameEvents.onVesselGoOnRails.Add(new EventData<Vessel>.OnEvent(CalculateElectricalData));
      GameEvents.onVesselWasModified.Add(new EventData<Vessel>.OnEvent(CalculateElectricalData));

      FindDataManager();
      if (Settings.DebugMode)
      {
          Utils.Log(String.Format("Initialization completed with buffer scale {0} and timewarp limit {1}", bufferScale, timeWarpLimit));
      }
    }

    protected override void OnSave(ConfigNode node)
    {
      // Saving needs to trigger a buffer clear
      ClearBufferStorage();
      base.OnSave(node);
    }

    void OnDestroy()
    {
      GameEvents.onVesselDestroy.Remove(CalculateElectricalData);
      GameEvents.onVesselGoOnRails.Remove(CalculateElectricalData);
      GameEvents.onVesselWasModified.Remove(CalculateElectricalData);
    }

    void FindDataManager()
    {
      vesselData = vessel.GetComponent<VesselDataManager>();
      if (!vesselData)
      {
        Utils.Error(LogVessel("Could not find vessel data manager"));
      }
    }

    void FixedUpdate()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {

        if (!vesselLoaded && FlightGlobals.ActiveVessel == vessel)
        {
          FindDataManager();
          CalculateElectricalData();
          vesselLoaded = true;
        }
        if (vesselLoaded && FlightGlobals.ActiveVessel != vessel)
        {
          vesselLoaded = false;
        }

        if (TimeWarp.CurrentRate < timeWarpLimit)
        {
          analyticMode = false;
          DoLowWarpSimulation();

        } else
        {
          analyticMode = true;
          DoHighWarpSimulation();
        }
      }
    }

    /// <summary>
    /// Runs the compensation at low warp. This typically means do nothing
    /// </summary>
    protected void DoLowWarpSimulation()
    {
      if (bufferStorage != null && bufferStorage.maxAmount != originalMax)
      {
        bufferStorage.maxAmount = originalMax;
      }
    }

    /// <summary>
    /// Runs the compensation at high warp
    /// </summary>
    protected void DoHighWarpSimulation()
    {
      if (vesselData != null && vesselData.ElectricalData != null)
      {
        double production = vesselData.ElectricalData.CurrentProduction;
        double consumption = vesselData.ElectricalData.CurrentConsumption;
        CalculateBuffer(production, consumption);
      }
    }

    /// <summary>
    /// Calculates the required buffer and applies it
    /// </summary>
    protected void CalculateBuffer(double production, double consumption)
    {
      float powerNet = (float)(production + consumption);

      if (powerNet < 0d)
      {
        // In this case, power generation is too low to handle draw, so no buffer is required
        if (bufferStorage != null)
        {
          if (Settings.DebugMode)
            Utils.Log(LogVessel("Power production too low, clearing buffer"));
          ClearBufferStorage();
        }
      }
      else
      {
        // Buffer size should be equal to one physics frame of consumption with an appropriate fudge factor
        bufferSize = Math.Abs(consumption) * (double)TimeWarp.fixedDeltaTime * bufferScale;

        if (bufferStorage != null)
        {
          // Calculate the difference between a required buffer size and the vessel maximum EC
          bufferDifference = (double)Mathf.Clamp((float)(bufferSize - totalEcMax), 0f, 9999999f);

          if (Settings.DebugMode)
          {
            Utils.Log(LogVessel(String.Format("Buffer needs {0:F2} EC space to reach target amount of {1:F2} EC", bufferDifference, originalMax + bufferDifference )));
          }

          // Apply the buffer
          bufferStorage.amount = (double)Mathf.Clamp((float)bufferStorage.amount, 0f, (float)(originalMax + bufferDifference));
          bufferStorage.maxAmount = originalMax + bufferDifference;
        }
      }
    }

    /// <summary>
    /// Calculates all required electrical data elements
    /// </summary>
    protected void CalculateElectricalData()
    {
      if (Settings.DebugMode)
        Utils.Log(LogVessel("Regenerating electrical data"));
      if (vessel == null || vessel.Parts == null)
      {
          if (Settings.DebugMode)
            Utils.Log(LogVessel("Refresh of electrical data failed for vessel, not initialized"));
          return;
      }
      // If the buffer does not exist, create it
      if (bufferPart == null)
      {
        double amount;
        double maxAmount;
        vessel.GetConnectedResourceTotals(PartResourceLibrary.ElectricityHashcode, out amount, out maxAmount);
        totalEcMax = maxAmount;
        CreateBufferStorage();
      }
      dataReady = true;
    }


    protected void CalculateElectricalData(Vessel eventVessel)
    {
      CalculateElectricalData();
    }
    protected void CalculateElectricalData(ConfigNode node)
    {
        CalculateElectricalData();
    }

    /// <summary>
    /// Clears the buffer (reverts it to its initial state)
    /// </summary>
    protected void ClearBufferStorage()
    {

      if (bufferStorage != null)
      {
        if (Settings.DebugMode)
          Utils.Log(LogVessel("Trying to clear buffer storage"));
        // Clamp the energy to the initial maximum and revert the maximum
        bufferStorage.amount = (double)Mathf.Clamp((float)bufferStorage.amount, 0f, (float)(originalMax));
        bufferStorage.maxAmount = originalMax;

        // Also do this to tbe ProtoResource
        foreach(ProtoPartResourceSnapshot proto in bufferPart.protoPartSnapshot.resources)
        {
          if (proto.resourceName == "ElectricCharge")
          {
            proto.amount = bufferStorage.amount;
            proto.maxAmount = originalMax;
          }
        }
      }
    }

    /// <summary>
    /// Creates the buffer and initializes the appropriate variables
    /// </summary>
    protected void CreateBufferStorage()
    {
      if (bufferPart != null)
      {
        if (Settings.DebugMode)
          Utils.Log(LogVessel(String.Format("Already has buffer on {0} with an initial capacity of {1:F1} EC", bufferPart.partInfo.name, originalMax)));
        return;
      }
      // Find a part containing electricity and set it as the buffer
      for (int i = 0; i < vessel.parts.Count; i++ )
      {
        if (vessel.parts[i].Resources.Contains("ElectricCharge"))
        {
          bufferPart = vessel.parts[i];
          bufferStorage = vessel.parts[i].Resources.Get("ElectricCharge");
          originalMax = bufferStorage.maxAmount;

          if (Settings.DebugMode)
            Utils.Log(LogVessel(String.Format("Created buffer on {0} with an initial capacity of {1} EC", vessel.parts[i].partInfo.name, originalMax)));
          return;
        }
      }
      if (Settings.DebugMode)
        Utils.Log(LogVessel(String.Format("Could not find an electrical storage part")));
    }

    /// <summary>
    /// Creates a log message that is prefixed by the vessel name
    /// </summary>
    protected string LogVessel(string msg)
    {
      return String.Format("[Controller] [{0}]: {1}",  vessel.name,msg);
    }
  }
}
