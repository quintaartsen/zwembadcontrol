
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using ZwembadControl.Models;
using HtmlAgilityPack;
using System.Globalization;

namespace ZwembadControl.Connectors
{
    public class HyconData
    {
        public double TargetTempature { get; set; }
        public double CurrentTempature { get; set; }
        public string TargetPh { get; set; }
        public string CurrentPh { get; set; }
        public string TargetChloor { get; set; }
        public string CurrentChloor { get; set; }
        public string TargetFlow { get; set; }
        public string CurrentFlow { get; set; }
    }

    public class HyconConnector
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string baseUrl = "http://192.168.10.241:8080";

        public async Task<HyconData> GetDataAsync()
        {
            string url = $"{baseUrl}/index.zhtml";

            try
            {
                var htmlContent = await client.GetStringAsync(url);

                if (string.IsNullOrEmpty(htmlContent))
                {
                    throw new Exception("De HTML-inhoud is leeg.");
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                var rows = doc.DocumentNode.SelectNodes("//table//tr");

                if (rows == null)
                {
                    throw new Exception("De 'Temp' rij is niet gevonden in de HTML.");
                }
                string setpointValue = default;
                string currentValue = default;


                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("td");

                    if (cells != null && cells.Count >= 4 && cells[1].InnerText.Trim() == "Temp")
                    {
                        setpointValue = cells[2].InnerText.Trim();
                        currentValue = cells[3].InnerText.Trim();
                    }
                    if (cells != null && cells.Count >= 4 && cells[1].InnerText.Trim() == "pH")
                    {
                        setpointValue = cells[2].InnerText.Trim();
                        currentValue = cells[3].InnerText.Trim();
                    }
                    if (cells != null && cells.Count >= 4 && cells[1].InnerText.Trim() == "Chloor")
                    {
                        setpointValue = cells[2].InnerText.Trim();
                        currentValue = cells[3].InnerText.Trim();
                    }
                    if (cells != null && cells.Count >= 4 && cells[1].InnerText.Trim() == "Flow")
                    {
                        setpointValue = cells[2].InnerText.Trim();
                        currentValue = cells[3].InnerText.Trim();
                    }
                }

                if (setpointValue == default || currentValue == default)
                {
                    throw new Exception("De temperatuurwaarden konden niet worden gevonden.");
                }

                if (!double.TryParse(setpointValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double targetTemp))
                {
                    throw new FormatException($"Kan de setpoint waarde '{setpointValue}' niet omzetten naar een geldig getal.");
                }

                if (!double.TryParse(currentValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double currentTemp))
                {
                    throw new FormatException($"Kan de current waarde '{currentValue}' niet omzetten naar een geldig getal.");
                }

                var hyconData = new HyconData
                {
                    TargetTempature = targetTemp,
                    CurrentTempature = currentTemp
                };

                return hyconData;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Fout bij het ophalen van de HTML-inhoud: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
            }
            return new HyconData
            {
                TargetTempature = default,
                CurrentTempature = default
            };
        }
    }
}