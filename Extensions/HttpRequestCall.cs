using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Extensions
{
    public interface IAtsHttpService
    {
        Task<HttpResponseMessage> MakeHttpRequestAsync(HttpRequestMessage httpRequestMessage, [CallerMemberName] string callerName = "");
    }

    public class HttpService : IAtsHttpService
    {
        //private readonly ICorrelationContextAccessor _cIdAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;

        public HttpService(ILogger<HttpService> logger, HttpClient httpClient)
        //public HttpService(ILogger<HttpService> logger, HttpClient httpClient, ICorrelationContextAccessor cIdAccessor)
        {
            _logger = logger;
            _httpClient = httpClient;
            //_cIdAccessor = cIdAccessor;
        }

        public async Task<HttpResponseMessage> MakeHttpRequestAsync(HttpRequestMessage httpRequestMessage, string callerName = "")
        {
            var requestBody = httpRequestMessage.Content != null ? await httpRequestMessage.Content.ReadAsStringAsync() : "No Response Body";
            //httpRequestMessage.Headers.Add(_cIdAccessor.CorrelationContext.CorrelationIdHeader, _cIdAccessor.CorrelationContext.CorrelationId);

            _logger.LogInformation("HTTP Request - Request - Method Caller: {Method}, HTTP Message: {Request}, with Request Body: {RequestBody}, Default Request Headers: {DefaultHeaders}",
                callerName, JsonConvert.SerializeObject(httpRequestMessage), requestBody, _httpClient.DefaultRequestHeaders.ToString());
            var response = await _httpClient.SendAsync(httpRequestMessage);

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"HTTP Request - Response Received - Method Caller: {callerName}, StatusCode:" +
                $" {response.StatusCode.ToString()}, RequestHeader: {response.Headers.ToString()}, Content: {content}");

            return response;
        }
    }
}
