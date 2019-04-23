using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DealerInsightRazor.Models;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace DealerInsightRazor.Authentication
{
    public class AuthenticationHandler
    {

        private readonly AzureAdSettings _azureSettings;
        private readonly PowerBiSettings _powerBiSettings;

        public AuthenticationHandler(IOptions<AzureAdSettings> azureOptions, IOptions<PowerBiSettings> powerBiOptions)
        {
            _azureSettings = azureOptions.Value;
            _powerBiSettings = powerBiOptions.Value;
        }

        /// <returns></returns>
        public async Task<(TokenCredentials tokenCredentials, string accessToken)> GetAzureTokenDataAsync()
        {

            //var authorize = $"{_azureSettings.Instance}{_azureSettings.TenantId}/oauth2/authorize?";
            //var oauthEndpoint = new Uri(authorize);

            //using (var client = new HttpClient())
            //{
            //    var result = await client.PostAsync(oauthEndpoint, new FormUrlEncodedContent(new[]
            //    {
            //        new KeyValuePair<string, string>("client_id", _azureSettings.ClientId),
            //        new KeyValuePair<string, string>("response_type", "code"),
            //        new KeyValuePair<string, string>("redirect_uri", "http://localhost:53189/"),
            //        new KeyValuePair<string, string>("resource", _powerBiSettings.ResourceAddress)
            //    }));

            //    var content = await result.Content.ReadAsStringAsync();

            //    var authenticationResult = JsonConvert.DeserializeObject<OAuthResult>(content);
            //    return (new TokenCredentials(authenticationResult.AccessToken, "Bearer"), authenticationResult.AccessToken);
            //}


            var authorityUrl = $"{_azureSettings.Instance}{_azureSettings.TenantId}/oauth2/token";

            var oauthEndpoint = new Uri(authorityUrl);

            using (var client = new HttpClient())
            {
                var result = await client.PostAsync(oauthEndpoint, new FormUrlEncodedContent(new[]
                {
                     new KeyValuePair<string, string>("resource", _powerBiSettings.ResourceAddress),
                     new KeyValuePair<string, string>("client_id", _azureSettings.ClientId),
                     new KeyValuePair<string, string>("grant_type", "authorization_code"),
                     new KeyValuePair<string, string>("username", _powerBiSettings.MasterUser),
                     new KeyValuePair<string, string>("password", _powerBiSettings.MasterKey),
                     new KeyValuePair<string, string>("scope", "openid"),
                 }));

                var content = await result.Content.ReadAsStringAsync();

                var authenticationResult = JsonConvert.DeserializeObject<OAuthResult>(content);
                return (new TokenCredentials(authenticationResult.AccessToken, "Bearer"), authenticationResult.AccessToken);
            }
        }
    }
}