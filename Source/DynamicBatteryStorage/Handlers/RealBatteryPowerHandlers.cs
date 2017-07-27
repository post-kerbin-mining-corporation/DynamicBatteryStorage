
namespace DynamicBatteryStorage
{

    // RealBattery: Realistic behaving batteries; https://github.com/blackliner/RealBattery
    public class RealBatteryHandler : PowerHandler
    {
        public override double GetPower()
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
