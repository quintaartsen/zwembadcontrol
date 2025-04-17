using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
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
        [Route("StartL")]
        public async void Startl()
        {
            await _service.StartLegionellasync();
        }

        [HttpGet]
        [Route("Stopl")]
        public async void Stopl()
        {
            await _service.StartLegionellasync();
        }

        [HttpGet]
        [Route("GetStateHistory")]
        public List<DateModel> GetStateHistory() => _service.GetStateHistory();

        [HttpGet]
        [Route("GetDouchData")]
        public async Task<string> GetDouchData()
        {
            string url = "http://192.168.172.102/settings/all.xml";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send an HTTP GET request to the URL
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Check if the response was successful (status code 200-299)
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Successfully fetched data:");
                        Console.WriteLine(content);  // Print the content of the XML
                        return content;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch data. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return default;
        }

        [HttpGet]
        [Route("StartSpoelen")]
        public async Task<string> StartSpoelen()
        {
            var url = "http://192.168.172.102/settings/Cycle.xml";
            var client = new HttpClient();

            // Prepare content with correct content type
            var content = new StringContent("2", Encoding.UTF8, "text/plain");

            try
            {
                var response = await client.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Response Status: " + response.StatusCode);
                Console.WriteLine("Response Body:");
                Console.WriteLine(responseBody);
                return response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
            return default;
        }


        [HttpGet]
        [Route("test")]
        public async Task<List<string>> Test()
        {
            List<string> result = new List<string>();
            List<Task> tasks = new List<Task>();  // Lijst van taken om alle verzoeken gelijktijdig te starten

            using (HttpClient client = new HttpClient())
            {
                // Loop door alle IP-adressen van 192.168.0.0 tot 192.168.254.254
                for (int i = 0; i <= 254; i++)
                {
                    for (int j = 0; j <= 254; j++)
                    {
                        string ip = $"192.168.{i}.{j}";
                        string url = $"http://{ip}/settings/all.xml";

                        // Start een nieuwe taak voor elke URL
                        tasks.Add(CheckUrlAsync(client, url, ip, result));
                    }
                }

                // Wacht totdat alle taken zijn voltooid
                await Task.WhenAll(tasks);
            }

            return result;
        }

        private async Task CheckUrlAsync(HttpClient client, string url, string ip, List<string> result)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                // Controleer of de aanvraag succesvol was
                if (response.IsSuccessStatusCode)
                {
                    lock (result)  // Zorg ervoor dat de toegang tot de lijst thread-safe is
                    {
                        result.Add(ip);
                    }
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
