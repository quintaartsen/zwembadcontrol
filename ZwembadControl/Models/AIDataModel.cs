using Tibber.Sdk;

namespace ZwembadControl.Models
{
    public class AIDataModel
    {

        public DateTime CurrentDateTime { get; set; }
        public double CurrentHotWaterTemp { get; set; }
        public double TargetHotWaterTemp { get; set; }
        public int CurrentZwembadWaterTemp { get; set; }
        public int TargetZwembadWaterTemp { get; set; }
        public PriceLevel CurrentPriceLevel { get; set; }

        public override string ToString()
        {
            return $"DateTime: {CurrentDateTime}, " +
                   $"HotWaterTemp: {CurrentHotWaterTemp}°C / {TargetHotWaterTemp}°C, " +
                   $"ZwembadTemp: {CurrentZwembadWaterTemp}°C / {TargetZwembadWaterTemp}°C, " +
                   $"Price Level: {CurrentPriceLevel}";
        }
    }
}
