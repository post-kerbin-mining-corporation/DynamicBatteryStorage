
namespace DynamicBatteryStorage
{

    // RealBattery: Realistic behaving batteries; https://github.com/blackliner/RealBattery
    public class RealBatteryPowerHandler : ModuleDataHandler
    {
        public override double GetValue()
        {
            double results = 0d;
            double.TryParse(pm.Fields.GetValue("lastECpower").ToString(), out results);
            return results * -1.0d; // positive value means the battery is charging --> consuming EC; thus negating result
        }
        public override bool IsProducer()
        {
            return GetPower() > 0;
        }
    }
}
