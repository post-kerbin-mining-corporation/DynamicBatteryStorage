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
    public double SavedMaxEC { get { return originalMax; } }
    public double SavedVesselMaxEC { get { return totalEcMax; } }

    #region PrivateVariables

    float timeWarpLimit = 100f;
    double bufferScale = 1.5d;

    bool vesselLoaded = false;
    bool analyticMode = false;
    bool dataReady = false;

    double bufferSize;

    double originalMax = 0d;
    double totalEcMax = 0d;

    VesselDataManager vesselData;
    PartResource bufferStorage;
    Part bufferPart;

    #endregion

    protected override void  OnStart()
    {
      base.OnStart();

      bufferScale = (double)Settings.BufferScaling;
      timeWarpLimit = Settings.TimeWarpLimit;

      FindDataManager(;)
      if (Settings.DebugMode)
      {
          Utils.Log(String.Format("[ModuleDynamicBatteryStorage]: Initialization completed with buffer scale {0} and timewarp limit {1}", bufferScale, timeWarpLimit));
      }
    }

    protected override void OnSave(ConfigNode node)
    {
      // Saving needs to trigger a buffer clear
      ClearBufferStorage();
      base.OnSave(node);
    }

    void FindDataManager()
    {
      dataManager = vessel.GetComponent<VesselDataManager>();
      if (!dataManager)
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
    protected void DoLowWarpSimulation()
    {
      if (bufferStorage != null && bufferStorage.maxAmount != originalMax)
      {
        bufferStorage.maxAmount = originalMax;
      }
    }
    protected void DoHighWarpSimulation()
    {
      if (powerHandlers.Count > 0)
      {

        double production = 0d;
        double consumption = 0d;

        for (int i=0; i < powerHandlers.Count; i++)
        {
          double pwr = powerHandlers[i].GetPower();
          if (pwr > 0d)
            production += pwr;
          else
            consumption += -pwr;
        }
        AllocatePower(production, consumption);
      }
    }

    protected void AllocatePower(double production, double consumption)
    {
      // Debug.Log(String.Format("P: {0} C: {1}", production, consumption));
      // normalize this
      consumption = consumption * 1d;

      float powerNet = Mathf.Clamp((float)(production - consumption), -9999999f, 0f);

      if (powerNet < 0d)
      {
        if (bufferStorage != null)
        {
          Utils.Log(LogVessel("Power production too low, clearing buffer"));
          ClearBufferStorage();
        }
      }
      else
      {
        bufferSize = consumption * (double)TimeWarp.fixedDeltaTime * bufferScale;
        if (bufferStorage != null)
        {
          double delta = (double)Mathf.Clamp((float)(bufferSize - totalEcMax), 0f, 9999999f);
          if (DebugMode) {
          Utils.Log(String.Format("delta {0}, target amt {1}", delta, originalMax+delta ));
        }
        bufferStorage.amount = (double)Mathf.Clamp((float)bufferStorage.amount, 0f, (float)(originalMax + delta));
        bufferStorage.maxAmount = originalMax + delta;
        }
      }
    }




    if (powerHandlers.Count > 0)
    {
    double amount;
    double maxAmount;

    if (bufferPart != null)
    {

    } else {
    vessel.GetConnectedResourceTotals(PartResourceLibrary.ElectricityHashcode, out amount, out maxAmount);
    totalEcMax = maxAmount;

    CreateBufferStorage();
    }

    }
    if (vessel.loaded)
    {
    Utils.Log(String.Format("Summary: \n vessel {0} (loaded state {1})\n" +
    "- {2} stock power handlers", vessel.name, vessel.loaded.ToString(), powerHandlers.Count));
    }

    dataReady = true;
    }



    protected void ClearBufferStorage()
    {

      if (bufferStorage != null)
      {
        Utils.Log(LogVessel("Trying to clear buffer storage"));
        bufferStorage.amount = (double)Mathf.Clamp((float)bufferStorage.amount, 0f, (float)(originalMax));
        bufferStorage.maxAmount = originalMax;

        //Debug.Log(String.Format("{0}, {1}", bufferStorage.amount, bufferStorage.maxAmount));
        foreach(ProtoPartResourceSnapshot proto in bufferPart.protoPartSnapshot.resources)
        {
          if (proto.resourceName == "ElectricCharge")
          {
            //Debug.Log(String.Format("{0}, {1}", proto.amount, proto.maxAmount));
            proto.amount = bufferStorage.amount;
            proto.maxAmount = originalMax;
            //Debug.Log(String.Format("{0}, {1}", proto.amount, proto.maxAmount));
          }
        }
      }
    }

    bool hasBuffer = false;
    protected void CreateBufferStorage()
    {
      if (bufferPart != null)
      {
        Utils.Log(String.Format("Has buffer on {0} with orig max {1}", bufferPart.partInfo.name, originalMax));


        return;
      }
      for (int i = 0; i < vessel.parts.Count; i++ )
      {
        if (vessel.parts[i].Resources.Contains("ElectricCharge"))
        {
          bufferPart = vessel.parts[i];
          bufferStorage = vessel.parts[i].Resources.Get("ElectricCharge");
          originalMax = bufferStorage.maxAmount;
          Utils.Log(String.Format("Located storage on {0} with {1} inital EC", vessel.parts[i].partInfo.name, originalMax));
          return;
        }
      }
      Utils.Log(String.Format("Could not find an electrical storage part on the vessel"));
    }


    protected LogVessel(string msg)
    {
      return String.Format("[ModuleDynamicBatteryStorage]: {0} on vessel {1}", msg, vessel.name);
    }


  }
}
