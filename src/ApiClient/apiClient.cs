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
        private const String baseUrl = "http://localhost:5011";

        public async Task<String> Post(String url, string gpu, string kiosk)
        {
            try	
            {
                var requestBody = new ApiRequest {url = url, gpu = gpu, kiosk = kiosk};
                var body = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                Console.WriteLine("Sending request body: " + body);

                HttpResponseMessage response = await client.PostAsync(baseUrl + "/url", content);
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

        public async Task<String> GetUrl()
        {
            return await this.BaseGet("url");
        }

        public async Task<String> GetGpu()
        {
            return await this.BaseGet("gpu");
        }

        public async Task<String> GetKiosk()
        {
            return await this.BaseGet("kiosk");
        }

        public async Task<String> GetVersion()
        {
            return await this.BaseGet("version");
        }

        public async Task<String> GetFlags()
        {
            return await this.BaseGet("flags");
        }

        private async Task<String> BaseGet(string endpoint)
        {
            try
            {
            HttpResponseMessage response = await client.GetAsync(baseUrl + "/" + endpoint);
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
    }

    public class ApiRequest
    {
        public string url { get; set;}
        public string gpu { get; set;}
        public string kiosk { get; set;}
    }
}