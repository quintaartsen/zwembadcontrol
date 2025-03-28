namespace ZwembadControl.Pages
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Threading.Tasks;
    using ZwembadControl.Connectors;
    using ZwembadControl.Models;

    public class IndexModel : PageModel
    {
        public string Mode { get; set; } = "Normaal";  // Standaard modus is 'Normaal'
        public string ZwembadKlepStatus { get; set; }
        public string BoilerKlepStatus { get; set; }
        public string ZwembadWarmtePomp { get; set; }
        public string AirwellWarmtePomp { get; set; }

        public double CurrentZwembadTemp { get; set; }
        public double TargetZwembadTemp { get; set; } 

        public double TargetBufferWaterTemp { get; set; }
        public double TargetBoilerWaterTemp { get; set; }
        public double CurrentBoilerWaterTemp { get; set; }


        public async Task OnGetAsync()
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
        }

        // Deze methode wordt aangeroepen wanneer de form wordt ingediend
        public IActionResult OnPost(string Mode, string KlepStatus, string BoilerKlepStatus)
        {
            
            return Page();  // Herlaad de pagina met de nieuwe gegevens
        }
    }
}
