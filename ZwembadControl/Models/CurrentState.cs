using Tibber.Sdk;

namespace ZwembadControl.Models
{
    public class DateModel
    {
        public double CurrentBoilerWaterTemp { get; set; }
        public double TargetBoilerWaterTemp { get; set; }
        public double TargetBufferWaterTemp { get; set; }
        public double CurrentZwembadWaterTemp { get; set; }
        public double TargetZwembadWaterTemp { get; set; }
        public string CurrentPriceLevel { get; set; }

        public bool ZwembadWarmtePomp { get; set; }
        public bool AirwellWarmtePomp { get; set; }
        public bool ZwembadKlepOpen { get; set; }
        public bool LegionellaBoiler { get; set; }
        public bool BoilerKlepOpen { get; set; }

        public DateTime currentDateTime { get; set; }
    }

    public class CurrentState
    {
        private static DateModel _instance;

        private static readonly object _lock = new object();

        public static void Reset(DateModel dateModel)
        {
            _instance = dateModel;
        }

        public static DateModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DateModel();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
