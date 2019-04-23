using System;
using System.Collections.Generic;
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
            var authorityUrl = $"{_azureSettings.Instance}{_azureSettings.TenantId}/oauth2/token";

            var client = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", _powerBiSettings.MasterUser),
                new KeyValuePair<string, string>("password", _powerBiSettings.MasterKey),
                new KeyValuePair<string, string>("client_id",  _azureSettings.ClientId),
                new KeyValuePair<string, string>("scope", "openid"),
                new KeyValuePair<string, string>("resource", _powerBiSettings.ResourceAddress)
            });

            var accessToken =
                await client.PostAsync(authorityUrl, content)
                    .ContinueWith<string>((response) =>
                    {
                        var authenticationResult = JsonConvert.DeserializeObject<OAuthResult>(response.Result.Content.ReadAsStringAsync().Result);
                        return authenticationResult?.AccessToken;
                    });

            return (new TokenCredentials(accessToken, "Bearer"), accessToken);
        }
    }
}