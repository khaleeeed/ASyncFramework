using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model.Response;
using ASyncFramework.Infrastructure.Service.model.Request;
using ASyncFramework.Infrastructure.Service.model.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Service
{
    public class SendEmailService : ISendEmailService
    {
        private readonly AppConfiguration appConfiguration;
        private readonly IConfiguration _Configuration;
        public SendEmailService(IOptionsMonitor<AppConfiguration> options,IConfiguration configuration)
        {
            appConfiguration = options.Get(AppConfiguration.EmailConfirmationApi);
            _Configuration = configuration;
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
            var accessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenModel>(jsonResponse);
            if (!responseMessage.IsSuccessStatusCode)
                return null;

            return accessToken.Token;
        }

        public async Task<GenericServiceResponse<object>> SendConfirmationEmail(string userName, string systemName, string token)
        {          
            var url = $"{appConfiguration.ASyncHost}/api/account/confirmation?token={token}";
            using HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using HttpClient client = new HttpClient(clientHandler);

            string apigeeApiAccessToken = await GetAccessTokenAsync();
            if (apigeeApiAccessToken is null)
            {
                return new GenericServiceResponse<object> { IsSuccessful = false, ResponseMessage = "Cannot get access token" };
            }

            var listOfNotifyPeople = _Configuration.GetSection("listOfNotifyPeople").Get<string []>();
            var request = new NotifyReq
            {
                TemplateId = 2550,
                EmailModel = new EmailModel
                {
                    Subject = $"Register {userName} new account in ASyncFramework",
                    CCTo = listOfNotifyPeople,
                    IsBodyHTML = true
                },
                SendByMedium = MediumTypes.Email,
                SendTo = new SendTo { EmailAddress = $"integrationTeam@moj.gov.sa" },
                RefrenceData = new Dictionary<string, string>
                {
                    { "userName", userName},
                    { "systemName", systemName },
                    { "url", url }
                }
            };

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " +  apigeeApiAccessToken );
            HttpResponseMessage responseMessage = await client.PostAsync(appConfiguration.Host, stringContent);

            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
            var userADService = Newtonsoft.Json.JsonConvert.DeserializeObject<GenericServiceResponse<object>>(jsonResponse);
            if (!responseMessage.IsSuccessStatusCode)
                return new GenericServiceResponse<object> { IsSuccessful = false, ResponseMessage = jsonResponse };

            return new GenericServiceResponse<object> { IsSuccessful = userADService.IsSuccessful, ResponseMessage = userADService.ResponseMessage };
        }
    }
}