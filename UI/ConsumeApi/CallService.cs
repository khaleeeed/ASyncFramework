using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UI.ConsumeApi
{
    public class CallService : ICallService
    {
        public CallService(IConfiguration configuration)
        {
            _BaseUrl = configuration.GetValue<string>("ManagerUrl");
        }

        private readonly string _BaseUrl;
        public async Task<T> CallGet<T>(string url, string token = null)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var httpResponseMessage = await client.GetAsync($"{_BaseUrl}/{url}");
            var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
            if(typeof(T) == typeof(string))
            {
                var obj= Convert.ChangeType(contentString, typeof(string));
                return (T)obj;      
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contentString);
        }
        public async Task<T> CallPost<T>(string url, string body,string token = null)
        {            
            using var client = new HttpClient();
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token}");
            var httpResponseMessage = await client.PostAsync($"{_BaseUrl}/{url}",content);
            var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
            if (typeof(T) == typeof(string))
            {
                var obj = Convert.ChangeType(contentString, typeof(string));
                return (T)obj;
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contentString);
        }

    }
}
