using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet]
        [Route("test")]
        public async void test()
        {
            using (HttpClient client = new HttpClient())
            {

                List<string> result = new List<string>();
                // Loop door alle IP-adressen van 192.168.0.0 tot 192.168.254.254
                for (int i = 0; i <= 254; i++)
                {
                    for (int j = 0; j <= 254; j++)
                    {
                        string ip = $"192.168.{i}.{j}";
                        string url = $"http://{ip}/settings/all.xml";

                        // Probeer de data op te halen
                        try
                        {
                            HttpResponseMessage response = await client.GetAsync(url);

                            // Controleer of de aanvraag succesvol was
                            if (response.IsSuccessStatusCode)
                            {
                                result.Add(ip);
                                Console.WriteLine($"Succesvol opgehaald: {url}");
                            }
                            else
                            {
                                Console.WriteLine($"Fout bij ophalen: {url} - Statuscode: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Fout bij verbinden met {url}: {ex.Message}");
                        }
                    }
                }
            }
    }
}
