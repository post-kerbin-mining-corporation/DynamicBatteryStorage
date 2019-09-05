using System;
using System.Reflection;

namespace DynamicBatteryStorage
{
    /// <summary>
    /// Support for RemoteTech mod at https://github.com/RemoteTechnologiesGroup/RemoteTech
    /// PartModule and Header configurations added to DynamicBatteryStorageSettings.cfg
    /// </summary>
    public class RemoteTechPowerHandler : ModuleDataHandler
    {
        PartModule antennaModule = null;

        public RemoteTechPowerHandler(HandlerModuleData moduleData) : base(moduleData)
        {
        }

        public override bool Initialize(PartModule pm)
        {
            base.Initialize(pm);
            antennaModule = pm;
            return true;
        }

        protected override double GetValueEditor()
        {
            return GetValueFlight();
        }

        protected override double GetValueFlight()
        {
            float consumerAmount = 0.0f;
            if (this.antennaModule != null)
            {
                PropertyInfo pi = this.antennaModule.GetType().GetProperty("Consumption");
                consumerAmount = (pi == null) ? 0.0f : (float)pi.GetValue(this.antennaModule, null);

                //additional information for future
                //float energyCost = 0.0f;
                //bool isBroken = false;
                //bool isActive = false;
                //float consumptionMultipler = 1.0f;

                //float.TryParse(this.antennaModule.Fields.GetValue("EnergyCost").ToString(), out energyCost);
                //bool.TryParse(this.antennaModule.Fields.GetValue("IsRTBroken").ToString(), out isBroken);
                //bool.TryParse(this.antennaModule.Fields.GetValue("IsRTActive").ToString(), out isActive);
                //float.TryParse(this.antennaModule.Fields.GetValue("ConsumptionMultiplier").ToString(), out consumptionMultipler); //do not work due not being KSPField
                //use reflection to get that non-KSPField property

                //consumerAmount = isBroken ? 0.0f : isActive ? (float)(energyCost * consumptionMultipler) : 0.0f;
            }
            return consumerAmount;
        }
    }
}
