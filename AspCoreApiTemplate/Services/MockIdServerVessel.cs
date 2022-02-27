using AspCoreApiTemplate.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspCoreApiTemplate.Services
{
    /// <summary>
    /// This is just a TEMPORARY class/process that mimics requests being sent to the back end using Authorization/Authentication
    /// DELETE when no longer needed
    /// </summary>
    public class MockIdServerVessel<T> : IMockIdServerVessel<T>
    {
        private AppSettings _appSettings;
        private static HttpClient _httpClient = null;
        private readonly ILogger<MockIdServerVessel<T>> _logger;

        public MockIdServerVessel(IOptions<AppSettings> appSettings, ILogger<MockIdServerVessel<T>> logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
            }
        }

        public async Task<IEnumerable<T>> SendMockGetRequest(string atsUrl)
        {
            //var oAuthToken = await GenerateTokenWithUrlAsync(clientIdValue, clientSecretValue, tokenUrlValue, grantType, scopeValue, usernameValue, passwordValue);
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oAuthToken);

            try
            {
                using (var response = await _httpClient.GetAsync(atsUrl))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var returnValue = JsonConvert.DeserializeObject<IEnumerable<T>>(content);
                    return returnValue;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error SendMockGetRequest making GetAsync.");
            }

            return null;
        }

        public async Task<T> SendMockPutRequest(T requestBody, string atsUrl)
        {
            //var oAuthToken = await GenerateTokenWithUrlAsync(clientIdValue, clientSecretValue, tokenUrlValue, grantType, scopeValue, usernameValue, passwordValue);
            var serializedNotification = JsonConvert.SerializeObject(requestBody);
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oAuthToken);

            try
            {
                var putRequest = new StringContent(serializedNotification, System.Text.Encoding.UTF8, "application/json");
                var notificationString = await putRequest.ReadAsStringAsync();//just used for debugging

                using (HttpResponseMessage response = await _httpClient.PutAsync(atsUrl, putRequest))
                {
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    var returnValue = JsonConvert.DeserializeObject<T>(responseData);
                    return (T)Convert.ChangeType(returnValue, typeof(T));
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error MockIdServerVessel making SendMockPutRequest.");
            }

            return (T)Convert.ChangeType(null, typeof(T));
        }

        //public async Task<string> GenerateTokenAsync(string clientId, string clientSecret, string atsUrl, string grantType, string scope, string username, string password)
        //{
        //    var atsResponseUrl = atsUrl;
        //    var grantCredentials = string.Empty;

        //    try
        //    {
        //        var tokenAuthInfo = "client_id=" + clientId;

        //        if (string.IsNullOrWhiteSpace(atsUrl))
        //        {
        //            atsResponseUrl = _appSettings.IdentityServerUri;
        //        }
        //        if (!string.IsNullOrWhiteSpace(clientSecret))
        //        {
        //            tokenAuthInfo += "&client_secret=" + clientSecret;
        //        }
        //        if (!string.IsNullOrWhiteSpace(grantType))
        //        {
        //            tokenAuthInfo += "&grant_type=" + grantType;
        //        }
        //        if (!string.IsNullOrWhiteSpace(scope))
        //        {
        //            tokenAuthInfo += "&scope=" + scope;
        //        }
        //        if (!string.IsNullOrWhiteSpace(username))
        //        {
        //            tokenAuthInfo += "&username=" + username;
        //        }
        //        if (!string.IsNullOrWhiteSpace(password))
        //        {
        //            tokenAuthInfo += "&password=" + password;
        //        }

        //        var buffer = System.Text.Encoding.UTF8.GetBytes(tokenAuthInfo);
        //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(buffer));
        //        var byteContent = new ByteArrayContent(buffer);
        //        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        //        using (var response = await _httpClient.PostAsync(atsResponseUrl, byteContent))
        //        {
        //            response.EnsureSuccessStatusCode();
        //            var results = await response.Content.ReadAsStringAsync();
        //            var token = JsonConvert.DeserializeObject<Token>(results);//Need to make a Token Object, if using this
        //            return token.AccessToken;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "GenerateTokenAsync Exception");
        //        throw ex;
        //    }
        //}
    }

    public interface IMockIdServerVessel<T>
    {
        Task<IEnumerable<T>> SendMockGetRequest(string atsUrl);
        Task<T> SendMockPutRequest(T requestBody, string atsUrl);
    }
}
