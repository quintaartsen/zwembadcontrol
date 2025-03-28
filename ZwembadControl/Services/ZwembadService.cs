using Microsoft.AspNetCore.Mvc;
using ZwembadControl.Services;

namespace ZwembadControl.Controllers
{
    public class ZwembadService
    {
        private readonly IKlepService _klepConnector;

        private double CurrentTempZwembad;
        private double TargetTempZwembad;

        private double CurrentTempBuffer;
        private double TargetTempBuffer;

        private double CurrentTempBoiler;
        private double TargetTempBoiler;


        public void run()
        {
            if (CurrentTempBuffer > 2)
            {

            }


        }


        public void StartSpoelen()
        {
            _klepConnector.CloseBoiler();
            _klepConnector.StartLegionellaBoiler();
        }

        public void StopSpoelen()
        {
            _klepConnector.CloseBoiler();
            _klepConnector.StopLegionellaBoiler();
        }
    }
}
