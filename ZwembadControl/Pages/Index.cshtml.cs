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
        public string Spoelen { get; set; }
        [BindProperty]
        public string ZwembadKlepMode { get; set; }
        [BindProperty]
        public string ZwembadMode { get; set; }
        [BindProperty]
        public string ZwembadPompMode { get; set; }
        [BindProperty]
        public string airwellMode { get; set; }
        [BindProperty]
        public string klimaatMode { get; set; }
        [BindProperty]
        public string boilerMode { get; set; }


        public string ZwembadKlepStatus { get; set; }
        public string BoilerKlepStatus { get; set; }
        public string ZwembadWarmtePomp { get; set; }
        public string AirwellWarmtePomp { get; set; }

        public double CurrentZwembadTemp { get; set; }
        public double TargetZwembadTemp { get; set; }
        public string TargetZwembadWaterPH { get; set; }
        public string CurrentZwembadWaterPH { get; set; }
        public string TargetZwembadWaterChloor { get; set; }
        public string CurrentZwembadWaterChloor { get; set; }
        public string TargetZwembadWaterFlow { get; set; }
        public string CurrentZwembadWaterFlow { get; set; }


        public double TargetBufferWaterTemp { get; set; }
        public double TargetBoilerWaterTemp { get; set; }
        public double CurrentBoilerWaterTemp { get; set; }

        public string CurrentPriceLevel { get; set; }


        private readonly ZwembadServiceActies _service;

        public IndexModel(ZwembadServiceActies zwembadservice)
        {
            _service = zwembadservice;
        }

        public async Task OnGetAsync()
        {
            UpdateDataModel();
        }

        public async Task OnPostAsync()
        {
            if(Mode != CurrentState.Instance.Mode && Mode != null)
            {
                CurrentState.Instance.Mode = Mode;
            }

            if (Spoelen != CurrentState.Instance.Spoelen && Spoelen != null)
            {
                CurrentState.Instance.Spoelen = Spoelen;

                switch (Spoelen)
                {
                    case "aan":
                        await _service.StartLegionellasync();
                        break;
                    case "uit":
                        await _service.StopLegionellasync();
                        break;
                }
            }

            if (ZwembadKlepMode != CurrentState.Instance.ZwembadKlepMode && ZwembadKlepMode != null)
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

            if (boilerMode != CurrentState.Instance.boilerMode && boilerMode != null)
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

            if (ZwembadMode != CurrentState.Instance.ZwembadMode && ZwembadMode != null)
            {
                CurrentState.Instance.ZwembadMode = ZwembadMode;

                switch (ZwembadMode)
                {
                    case "open":
                        await _service.StartZwembadWarmtePompAsync();
                        break;
                    case "close":
                        await _service.StopZwembadWarmtePompAsync();
                        break;
                    case "auto":
                        break;
                }
            }

            if (ZwembadPompMode != CurrentState.Instance.ZwembadPompMode && ZwembadPompMode != null)
            {
                CurrentState.Instance.ZwembadPompMode = ZwembadPompMode;

                switch (ZwembadPompMode)
                {
                    case "open":
                        await _service.StartZwembadPompAsync();
                        break;
                    case "close":
                        await _service.StopZwembadPompAsync();
                        break;
                    case "auto":
                        break;
                }
            }


            if (airwellMode != CurrentState.Instance.airwellMode && airwellMode != null)
            {
                CurrentState.Instance.airwellMode = airwellMode;

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

            if (klimaatMode != CurrentState.Instance.klimaatMode && klimaatMode != null)
            {
                CurrentState.Instance.klimaatMode = klimaatMode;

                switch (klimaatMode)
                {
                    case "open":
                        await _service.StartKlimaatSysteemasync();
                        break;
                    case "close":
                        await _service.StopKlimaatSysteemasync();
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
            CurrentZwembadWaterChloor = CurrentState.Instance.CurrentZwembadWaterChloor;
            TargetZwembadWaterChloor = CurrentState.Instance.TargetZwembadWaterChloor;
            CurrentZwembadWaterPH = CurrentState.Instance.CurrentZwembadWaterPH;
            TargetZwembadWaterPH = CurrentState.Instance.TargetZwembadWaterPH;
            CurrentZwembadWaterFlow = CurrentState.Instance.CurrentZwembadWaterFlow;
            TargetZwembadWaterFlow = CurrentState.Instance.TargetZwembadWaterFlow;

            TargetBufferWaterTemp = CurrentState.Instance.TargetBufferWaterTemp;
            TargetBoilerWaterTemp = CurrentState.Instance.TargetBoilerWaterTemp;
            CurrentBoilerWaterTemp = CurrentState.Instance.CurrentBoilerWaterTemp;
            CurrentPriceLevel = CurrentState.Instance.CurrentPriceLevel;

            Mode = CurrentState.Instance.Mode;
            Spoelen = CurrentState.Instance.Spoelen;
            ZwembadKlepMode = CurrentState.Instance.ZwembadKlepMode;
            ZwembadMode = CurrentState.Instance.ZwembadMode;
            airwellMode = CurrentState.Instance.airwellMode;
            klimaatMode = CurrentState.Instance.klimaatMode;
            boilerMode = CurrentState.Instance.boilerMode;

        }
    }
}
