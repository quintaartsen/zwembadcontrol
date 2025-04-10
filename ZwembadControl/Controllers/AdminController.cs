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
        [Route("GetStateHistory")]
        public async Task<DateModel> GetStateHistory()
        {
            return CurrentState.Instance;
        }

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
    }
}
