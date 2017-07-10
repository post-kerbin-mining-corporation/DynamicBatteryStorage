using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DynamicBatteryStorage
{

  public class ModuleDynamicBatteryStorage : VesselModule
  {
        float timeWarpLimit = 100f;
        double bufferScale = 1.5d;

        bool vesselLoaded = false;
        bool analyticMode = false;
        bool dataReady = false;
        int partCount = -1;

        public bool AnalyticMode {get {return analyticMode;}}

        public Part BufferPart { get { return bufferPart; } }
        public PartResource BufferResource { get { return bufferStorage; } }

        public double BufferScale { get { return bufferScale; } }
        public double BufferSize { get { return bufferSize; } }
        public double SavedMaxEC { get { return originalMax; } }
        public double SavedVesselMaxEC { get { return totalEcMax; } }

        public double ShipPowerConsumption {
          get {

            double consumption = 0d;
            for (int i=0; i < powerHandlers.Count; i++)
            {
              double pwr = p.GetPower();
              if (pwr < 0d)
                consumption += prw
            }
            return consumption;
          }
        }
        public double ShipPowerProduction {
         get {
           double production = 0d;
           for (int i=0; i < powerHandlers.Count; i++)
           {
             double pwr = p.GetPower();
             if (pwr > 0d)
               production += pwr;

             }
             return production;
           }
        }
        public List<PowerHandler> ShipProducers
        {
          get
          {
            List<PowerHandler> handlers = new List<PowerHandler>();
            for (int i=0; i < powerHandlers.Count; i++)
            {
              if (powerHandlers[i].IsProducer)
              {
                handlers.Add(powerHandlers[i]);
              }
            }
            return handlers;
          }
        }
        public List<PowerHandler> ShipConsumers
        {
          get
          {
            List<PowerHandler> handlers = new List<PowerHandler>();
            for (int i=0; i < powerHandlers.Count; i++)
            {
              if (!powerHandlers[i].IsProducer)
              {
                handlers.Add(powerHandlers[i]);
              }
            }
            return handlers;
          }
        }

        double bufferSize;

        double originalMax = 0d;
        double totalEcMax = 0d;

        PartResource bufferStorage;
        Part bufferPart;

        List<PowerHandler> powerHandlers = new List<PowerHandler>();

        //public override VesselModule.Activation GetActivation()
        //{
            //return Activation..LoadedVessels;
        //}

        protected override void  OnStart()
        {
 	          base.OnStart();

              bufferScale = (double)Settings.BufferScaling;
              timeWarpLimit = Settings.TimeWarpLimit;

            GameEvents.onVesselDestroy.Add(new EventData<Vessel>.OnEvent(RefreshVesselElectricalData));
            GameEvents.onVesselGoOnRails.Add(new EventData<Vessel>.OnEvent(RefreshVesselElectricalData));
            GameEvents.onVesselWasModified.Add(new EventData<Vessel>.OnEvent(RefreshVesselElectricalData));

            RefreshVesselElectricalData();
        }

        protected override void OnSave(ConfigNode node)
        {
            ClearBufferStorage();
            base.OnSave(node);
        }

        void OnDestroy()
        {
          GameEvents.onVesselDestroy.Remove(RefreshVesselElectricalData);
          GameEvents.onVesselGoOnRails.Remove(RefreshVesselElectricalData);
          GameEvents.onVesselWasModified.Remove(RefreshVesselElectricalData);
        }
        void FixedUpdate()
        {
          if (HighLogic.LoadedSceneIsFlight && dataReady)
          {
             // Debug.Log(String.Format("BufferStorage: Vessel {0}, loaded state is {1}",  vessel.name, vessel.loaded.ToString()));
              if (!vesselLoaded && FlightGlobals.ActiveVessel == vessel)
              {
                  //Debug.Log("Vessel changed state from unfocused to focused");
                  RefreshVesselElectricalData();
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
              double pwr = p.GetPower();
              if (pwr > 0d)
                production += pwr;
              else
                consumption += prw
            }
            AllocatePower(production, consumption);
          }
        }

        protected void AllocatePower(double production, double consumption)
        {
          // normalize this
          consumption = consumption * 1d;

          float powerNet = Mathf.Clamp((float)(production - consumption), -9999999f, 0f);

          if (powerNet < 0d)
          {
            if (bufferStorage != null)
            {
              ClearBufferStorage();
            }
          }
          else
          {
            bufferSize = consumption * (double)TimeWarp.fixedDeltaTime * bufferScale;
            if (bufferStorage != null)
            {
                double delta = (double)Mathf.Clamp((float)(bufferSize - totalEcMax), 0f, 9999999f);
                //Debug.Log(String.Format("delta {0}, target amt {1}", delta, originalMax+delta ));
                bufferStorage.amount = (double)Mathf.Clamp((float)bufferStorage.amount, 0f, (float)(originalMax + delta));
                bufferStorage.maxAmount = originalMax + delta;
            }
          }
        }

        protected void RefreshVesselElectricalData(Vessel eventVessel)
        {
          //Utils.Log("Refreshing data from Vessel event");
          RefreshVesselElectricalData();
        }
        protected void RefreshVesselElectricalData(ConfigNode node)
        {
            //Utils.Log("Refresh from save node event");
            RefreshVesselElectricalData();
        }
        protected void RefreshVesselElectricalData()
        {
          ClearElectricalData();
          if (vessel == null || vessel.Parts == null)
          {
              Utils.Log("Refresh failed for vessel, not initialized");
              return;
          }

          partCount = vessel.Parts.Count;

          for (int i = partCount - 1; i >= 0; --i)
          {
              Part part = vessel.Parts[i];
              for (int j = part.Modules.Count - 1; j >= 0; --j)
              {
                  PartModule m = part.Modules[j];
                  // Try to create accessor modules

                  SetupPowerHandler(m);
                  bool success = TrySetupProducer(m);

              }
            }

            if (powerConsumers.Count > 0)
            {
                double amount;
                double maxAmount;

                vessel.GetConnectedResourceTotals(PartResourceLibrary.ElectricityHashcode, out amount, out maxAmount);

                totalEcMax = maxAmount;

                CreateBufferStorage();
            }
            if (vessel.loaded)
            {
             Utils.Log(String.Format("Summary: \n vessel {0} (loaded state {1})\n" +
                "- {2} stock power producers \n" +
                "- {3} stock power consumers", vessel.name,vessel.loaded.ToString(), powerProducers.Count, powerConsumers.Count));
            }

          dataReady = true;
        }

        protected void ClearElectricalData()
        {
          powerProducers.Clear();
          powerConsumers.Clear();
          ClearBufferStorage();
        }

        protected void ClearBufferStorage()
        {
          //Utils.Log("Trying to clear buffer storage");
          if (bufferStorage != null)
          {
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

        protected void CreateBufferStorage()
        {
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

        protected void SetupPowerHandler(PartModule pm)
        {
          PowerHandlerType handlerType;
          if (TryParse<PowerProducerType>(pm.moduleName, out handlerType))
          {
            PowerHandler handler = (PowerHandler)System.Activator.CreateInstance(Type.GetType(pm.ModuleName + "Handler"));
            handler.Initialize(pm);
            powerHandlers.Add(pm);
          }
        }


        /// Checks to see whether a ModuleGenerator/ModuleResourceConverter/ModuleResourceHarvester is a producer or consumer
        protected bool VerifyInputs(PartModule pm, bool isProducer)
        {
          if (pm.moduleName == "ModuleResourceConverter" || pm.moduleName == "ModuleResourceHarvester")
          {
            BaseConverter conv = (BaseConverter)pm;
            if (isProducer)
            {
              for (int i = 0;i < conv.outputList.Count;i++)
                if (conv.inputList[i].ResourceName == "ElectricCharge")
                    return true;
              return false;
            } else
            {
                for (int i = 0; i < conv.inputList.Count; i++)
                    if (conv.inputList[i].ResourceName == "ElectricCharge")
                        return true;
              return false;
            }
          }
          if (pm.moduleName == "ModuleGenerator")
          {
            ModuleGenerator gen = (ModuleGenerator)pm;
            if (isProducer)
            {
              for (int i = 0; i < gen.resHandler.outputResources.Count; i++)
                  if (gen.resHandler.outputResources[i].name == "ElectricCharge")
                  {
                      return true;
                  }
              return false;
            } else
            {
              for (int i = 0; i < gen.resHandler.inputResources.Count; i++)
                  if (gen.resHandler.inputResources[i].name == "ElectricCharge")
                  {
                      return true;
                  }
              return false;
            }
          }
          return true;
        }


        public static bool TryParse<TEnum>(string value, out TEnum result)
      where TEnum : struct, IConvertible
        {
            var retValue = value == null ?
                        false :
                        Enum.IsDefined(typeof(TEnum), value);
            result = retValue ?
                        (TEnum)Enum.Parse(typeof(TEnum), value) :
                        default(TEnum);
            return retValue;
        }

  }
}
