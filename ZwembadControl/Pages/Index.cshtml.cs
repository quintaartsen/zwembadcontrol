namespace ZwembadControl.Pages
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Threading.Tasks;
    using ZwembadControl.Controllers;
    using ZwembadControl.Models;

    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Mode { get; set; }
        [BindProperty]
        public string ZwembadKlepMode { get; set; }
        [BindProperty]
        public string ZwembadMode { get; set; }
        [BindProperty]
        public string airwellMode { get; set; }
        [BindProperty]
        public string boilerMode { get; set; }


        public string ZwembadKlepStatus { get; set; }
        public string BoilerKlepStatus { get; set; }
        public string ZwembadWarmtePomp { get; set; }
        public string AirwellWarmtePomp { get; set; }

        public double CurrentZwembadTemp { get; set; }
        public double TargetZwembadTemp { get; set; } 

        public double TargetBufferWaterTemp { get; set; }
        public double TargetBoilerWaterTemp { get; set; }
        public double CurrentBoilerWaterTemp { get; set; }

        public string CurrentPriceLevel { get; set; }


        private readonly ZwembadService _service;

        public IndexModel(ZwembadService zwembadservice)
        {
            _service = zwembadservice;
        }

        public async Task OnGetAsync()
        {
            UpdateDataModel();
        }

        public async Task OnPostAsync()
        {
            ZwembadKlepStatus = ZwembadKlepMode;

            if(Mode != CurrentState.Instance.Mode)
            {
                CurrentState.Instance.Mode = Mode;
            }

            if (ZwembadKlepMode != CurrentState.Instance.ZwembadKlepMode)
            {
                CurrentState.Instance.ZwembadKlepMode = ZwembadKlepMode;

                switch (ZwembadKlepMode)
                {
                    case "open":
                        await _service.OpenZwembadKlepAsync();
                        break;
                    case "close":
                        await _service.CloseZwembadKlepAsync();
                        break;
                    case "auto":
                        break;
                }
            }

            if (boilerMode != CurrentState.Instance.boilerMode)
            {
                CurrentState.Instance.boilerMode = boilerMode;

                switch (boilerMode)
                {
                    case "open":
                        await _service.OpenBoilerKlepAsync();
                        break;
                    case "close":
                        await _service.CloseBoilerKlepAsync();
                        break;
                    case "auto":
                        break;
                }
            }

            if (ZwembadMode != CurrentState.Instance.ZwembadMode)
            {
                CurrentState.Instance.ZwembadMode = ZwembadMode;

                switch (ZwembadMode)
                {
                    case "open":
                        await _service.StartZwembadWarmtePompasync();
                        break;
                    case "close":
                        await _service.StopZwembadWarmtePompasync();
                        break;
                    case "auto":
                        break;
                }
            }

            if (airwellMode != CurrentState.Instance.airwellMode)
            {
                CurrentState.Instance.ZwembadMode = ZwembadMode;

                switch (ZwembadMode)
                {
                    case "open":
                        await _service.StartAirwellWarmtePompasync();
                        break;
                    case "close":
                        await _service.StopAirwellWarmtePompasync();
                        break;
                    case "auto":
                        break;
                }
            }

            UpdateDataModel();
        }

        private void UpdateDataModel()
        {
            ZwembadKlepStatus = CurrentState.Instance.ZwembadKlepOpen ? "Open" : "Dicht";
            BoilerKlepStatus = CurrentState.Instance.BoilerKlepOpen ? "Open" : "Dicht";
            ZwembadWarmtePomp = CurrentState.Instance.ZwembadWarmtePomp ? "Aan" : "Uit";
            AirwellWarmtePomp = CurrentState.Instance.AirwellWarmtePomp ? "Aan" : "Uit";

            CurrentZwembadTemp = CurrentState.Instance.CurrentZwembadWaterTemp;
            TargetZwembadTemp = CurrentState.Instance.TargetZwembadWaterTemp;
            TargetBufferWaterTemp = CurrentState.Instance.TargetBufferWaterTemp;
            TargetBoilerWaterTemp = CurrentState.Instance.TargetBoilerWaterTemp;
            CurrentBoilerWaterTemp = CurrentState.Instance.CurrentBoilerWaterTemp;
            CurrentPriceLevel = CurrentState.Instance.CurrentPriceLevel;
        }
    }
}
