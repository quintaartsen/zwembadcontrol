using System.Text;

namespace ZwembadControl.Connectors
{
    public class AcquaNetConnector
    {
        private readonly string baseUrl = "http://192.168.172.102";

        public async Task StartSpoelenAsync()
        {
            var url = $"{baseUrl}/settings/Cycle.xml";
            var client = new HttpClient();

            var content = new StringContent("2", Encoding.UTF8, "text/plain");

            try
            {
                var response = await client.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Response Status: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
        }

        public async Task<string> GetDataAsync()
        {
            string url = $"{baseUrl}/settings/all.xml";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Successfully fetched data:");
                        Console.WriteLine(content);
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
    }
}