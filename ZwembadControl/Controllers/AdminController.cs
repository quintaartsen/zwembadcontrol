using Microsoft.AspNetCore.Mvc;
using ZwembadControl.Connectors;
using ZwembadControl.Models;

namespace ZwembadControl.Controllers;

[Produces("application/json")]
[Route("api/v1/[controller]")]
[ApiController]
public class AdminController(ZwembadService service, ZwembadServiceActies _service, AcquaNetConnector acquaNetConnector) : Controller
{

    [HttpGet]
    [Route("GetCurrentStatus")]
    public async Task<DateModel> GetCurrentStatusAsync()
    {
        await service.RunJob();
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
        await _service.StartZwembadWarmtePompAsync();
    }

    [HttpGet]
    [Route("StopZwembadWarmtePompasync")]
    public async void StopZwembadWarmtePompasync()
    {
        await _service.StopZwembadWarmtePompAsync();
    }

    [HttpGet]
    [Route("GetStateHistory")]
    public List<DateModel> GetStateHistory() => service.GetStateHistory();

    [HttpGet]
    [Route("GetDouchData")]
    public async Task<string> GetDouchData()
    {
        return await acquaNetConnector.GetDataAsync();
    }

    [HttpGet]
    [Route("StartSpoelen")]
    public async Task StartSpoelenAsync()
    {
        await acquaNetConnector.StartSpoelenAsync();
    }

    [HttpGet]
    [Route("StartLegionella")]
    public async Task StartLegionellaAsync()
    {
        await _service.StartLegionellasync();
    }

    [HttpGet]
    [Route("StopLegionella")]
    public async Task StopLegionellaAsync()
    {
        await _service.StopLegionellasync();
    }
}
