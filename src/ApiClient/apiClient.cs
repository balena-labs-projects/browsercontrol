using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace src.ApiClient
{
    public class ApiClient
    {
        private readonly HttpClient client = new HttpClient();
        private String port = Environment.GetEnvironmentVariable("PORT") ?? "5011";

        public async Task<String> Post(String host, String url, string gpu, string kiosk)
        {
            try	
            {
                var requestBody = new UrlApiRequest {url = url, gpu = gpu, kiosk = kiosk};
                var body = JsonSerializer.Serialize(requestBody);
                return await BasePost(body, host, "url");
               
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
                return e.Message;
            }
        }

        public async Task<String> SetRefreshInterval(string host, int value)
        {
             try	
            {
                HttpResponseMessage response = await client.PostAsync(GetBaseUrl(host) + "/autorefresh/" + value, null);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseBody);
                return responseBody;
               
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
                return e.Message;
            }
        }

        public async Task<String> GetUrl(string host)
        {
            return await this.BaseGet(host, "url");
        }

        public async Task<String> GetGpu(string host)
        {
            return await this.BaseGet(host, "gpu");
        }

        public async Task<String> GetKiosk(string host)
        {
            return await this.BaseGet(host, "kiosk");
        }

        public async Task<String> GetVersion(string host)
        {
            return await this.BaseGet(host, "version");
        }

        public async Task<String> GetFlags(string host)
        {
            return await this.BaseGet(host, "flags");
        }

        private string GetBaseUrl(String host)
        {
            var hostValue = host ?? "localhost";
            return "http://" + hostValue + ":" + port;
        }

        private async Task<String> BaseGet(string host, string endpoint)
        {
            try
            {
            HttpResponseMessage response = await client.GetAsync(GetBaseUrl(host) + "/" + endpoint);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
                return e.Message;
            }
        }

        private async Task<String> BasePost(string body, string host, string endpoint)
            {
                
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                Console.WriteLine("Sending request body: " + body);

                HttpResponseMessage response = await client.PostAsync(GetBaseUrl(host) + "/" + endpoint, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseBody);
                return responseBody;
            }
    }

    public class UrlApiRequest
    {
        public string url { get; set;}
        public string gpu { get; set;}
        public string kiosk { get; set;}
    }
}