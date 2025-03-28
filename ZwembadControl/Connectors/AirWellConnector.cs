
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using ZwembadControl.Models;
using Newtonsoft.Json.Linq;

namespace ZwembadControl.Connectors
{
    public class AirWellConnector
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string baseUrl = "https://api-airhome.airwell.fr";
        private readonly string username = "opti1@live.nl";
        private readonly string password = "jk2dbQs4!";

        private string token;


        public async Task<string> LoginAsync()
        {
            string url = $"{baseUrl}/light/user/login";

            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;
                token = root.GetProperty("token").GetString();
                return token;
            }
            else
            {
                Console.WriteLine($"Login mislukt: {response.StatusCode}");
                return null;
            }
        }

        public async Task<AirWellData> GetDataAsync()
        {
            string url = $"{baseUrl}/light/data/15581/table";

            var requestData = new
            {
                distinctDevice = true,
                startDate = DateTime.UtcNow.AddMinutes(-15).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                endDate = DateTime.UtcNow.AddMinutes(15).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                deviceIds = new[] { 113255 },
                measurements = new[]
             {
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "scheduleSwitch", index = 0 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "silentFunctionSwitch", index = 1 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "holidaySwitch", index = 2 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "ecoFunctionSwitch", index = 3 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "wiredOperationMode", index = 4 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "temperatureZone1", index = 5 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "temperatureZone2", index = 6 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "hotWaterTemperature", index = 7 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "roomTemperature", index = 8 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "zone1HeatingTemperatureMaximum", index = 9 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "zone1HeatingTemperatureMinimum", index = 10 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "zone1CoolingTemperatureMaximum", index = 11 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "zone1CoolingTemperatureMinimum", index = 12 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "zone2HeatingTemperatureMaximum", index = 13 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "zone2HeatingTemperatureMinimum", index = 14 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "zone2CoolingTemperatureMaximum", index = 15 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "zone2CoolingTemperatureMinimum", index = 16 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "roomTemperatureMaximum", index = 17 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "roomTemperatureMinimum", index = 18 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "maximumTemperatureHotWater", index = 19 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "minimumTemperatureHotWater", index = 20 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "waterTankTemperature", index = 21 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "powerZone1", index = 22 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "powerZone2", index = 23 },
                new { operation = "last", table = "Airwell_MIDEA_CAC_Basic", column = "powerHotWater", index = 24 }
            }
            };

            var response = await Request(url, requestData);
            return JsonConvert.DeserializeObject<List<AirWellData>>(response)[0];
        }


        public async Task SwitchHotWater(bool value)
        {
            var requestData = new
            {
                encoderCommand = new
                {
                    id = 390,
                    parameters = new[]
                           {
                    new
                    {
                        key = "powerHotWater",
                        pathKey = "properties",
                        value = value
                    }
                }
                },
                deviceId = 113255
            };

            await ActionRequest(requestData);
        }

        public async Task SwitchHeating(bool value)
        {
            var requestData = new
            {
                encoderCommand = new
                {
                    id = 390,
                    parameters = new[]
                           {
                    new
                    {
                        key = "powerZone1",
                        pathKey = "properties",
                        value = value
                    }
                }
                },
                deviceId = 113255
            };

            await ActionRequest(requestData);
        }

        private async Task ActionRequest<T>(T requestData)
        {
            string url = $"{baseUrl}/light/message/15581/";
            await Request(url, requestData);
        }

        private async Task<string> Request<T>(string url, T requestData)
        {
            string jsonContent = System.Text.Json.JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await LoginAsync();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await client.PostAsync(url, content);
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            Console.WriteLine($"Fout bij ophalen van data: {response.StatusCode}");
            return default;
        }

        internal async Task SetTemp(int value)
        {
            var requestData = new
            {
                encoderCommand = new
                {
                    id = 390,
                    parameters = new[]
                           {
                    new
                    {
                        key = "hotWaterTemperature",
                        pathKey = "properties",
                        value = value
                    }
                }
                },
                deviceId = 113255
            };

            await ActionRequest(requestData);
        }
    }
}
