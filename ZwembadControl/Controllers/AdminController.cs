using Microsoft.AspNetCore.Mvc;
using ZwembadControl.Models;

namespace ZwembadControl.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly ZwembadService _service;

        public AdminController(ZwembadService zwembadService)
        {
            _service = zwembadService;
        }

        [HttpGet]
        [Route("GetCurrentStatus")]
        public async Task<DateModel> GetCurrentStatusAsync()
        {
            await _service.GetDataAsync();
            return CurrentState.Instance;
        }

        [HttpGet]
        [Route("OpenZwembadKlep")]
        public async void OpenZwembadKlep()
        {
            await _service.OpenZwembadKlepAsync();
        }

        [HttpGet]
        [Route("CloseZwembadKlep")]
        public async void CloseZwembadKlep()
        {
            await _service.CloseZwembadKlepAsync();
        }

        [HttpGet]
        [Route("OpenBoilerKlep")]
        public async void OpenBoilerKlep()
        {
            await _service.OpenBoilerKlepAsync();
        }

        [HttpGet]
        [Route("CloseBoilerKlep")]
        public async void CloseBoilerKlep()
        {
            await _service.CloseBoilerKlepAsync();
        }

        [HttpGet]
        [Route("StartZwembadWarmtePompasync")]
        public async void StartZwembadWarmtePompasync()
        {
            await _service.StartZwembadWarmtePompasync();
        }

        [HttpGet]
        [Route("StopZwembadWarmtePompasync")]
        public async void StopZwembadWarmtePompasync()
        {
            await _service.StopZwembadWarmtePompasync();
        }
    }
}
