using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model.Response;
using ASyncFramework.Infrastructure.Service.model.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Service
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly AppConfiguration appConfiguration;
        private readonly IUserRepository _UserRepository;
        public ActiveDirectoryService(IOptionsMonitor<AppConfiguration> options,IUserRepository userRepository)
        {
            appConfiguration = options.Get(AppConfiguration.ActiveDirectoryApi);
            _UserRepository = userRepository;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                   delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                   {
                       return true;
                   };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string credentials = $"{appConfiguration.UserName}:{appConfiguration.Password}";

            using HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using HttpClient client = new HttpClient(clientHandler);



            //Define Headers
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));
            //Prepare Request Body
            List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
            requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            // Add the grant-type to the request model
            FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);
            var responseMessage = await client.PostAsync(appConfiguration.HostToken, requestBody);
            //If success received   
            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<AccessTokenModel>(jsonResponse);
            if (!responseMessage.IsSuccessStatusCode)
                return null;

            return accessToken.Token;
        }

        public async Task<GenericServiceResponse<AsyncUser>> GetUserByUserName(string userName)
        {
            using HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using HttpClient client = new HttpClient(clientHandler);           
            
            var url = $"{appConfiguration.Host}s/details/{userName}";

            var token =await GetAccessTokenAsync();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            HttpResponseMessage responseMessage = await client.GetAsync(url);

            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
            var userADService = JsonConvert.DeserializeObject<UserADService>(jsonResponse);
            if (!responseMessage.IsSuccessStatusCode)
                return new GenericServiceResponse<AsyncUser> { IsSuccessful = false, ResponseMessage = jsonResponse };

            var user = _UserRepository.GetUser(userName);
            string role = null;
            if (user!=null)
            {
                role = $"{user.Role.RoleName}-{user.System.SystemCode}";
            }
            
            return new GenericServiceResponse<AsyncUser> { Data = new AsyncUser { Id = userADService.data.FirstOrDefault().user.EmployeeID, Roles =role, UserName =user.UserName }, IsSuccessful = true };
        }

        public async Task<GenericServiceResponse<AsyncUser>> Login(string userName, string password)
        {
            using HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using HttpClient client = new HttpClient(clientHandler);

            var token =await GetAccessTokenAsync();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var url = $"{appConfiguration.Host}/login";

            var jsonContent = JsonConvert.SerializeObject(new { username = userName, password });

            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await client.PostAsync(url, content);
            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
                return new GenericServiceResponse<AsyncUser> { IsSuccessful = false, ResponseMessage = jsonResponse };

            return new GenericServiceResponse<AsyncUser> { IsSuccessful = true };
        }

    }
} 