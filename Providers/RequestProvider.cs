using System.Net.Http;
using System.Text.Json;
using bekokkonen.pro.Global;
using bekokkonen.pro.Global.Config;

namespace bekokkonen.pro.Providers
{
    public sealed class RequestProvider(ILogger<RequestProvider> logger, IHttpClientFactory httpClientFactory) : IRequestProvider
    {
        private readonly string _serviceName = nameof(RequestProvider);
        private readonly ILogger<RequestProvider> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly Guid _operationId = Guid.NewGuid();

        public async Task PostAsync<TRequest>(string clientName, string url, TRequest data)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            try
            {
                PatchIfHeatHarmony(clientName, httpClient);

                var requestContent = SerializeToJson(data);
                _logger.LogInformation(
                    "{Timestamp} {ServiceName} {OperationId}:: serialization succeeded",
                    DateTime.Now, _serviceName, _operationId);

                using var response = await httpClient.PostAsync(url, requestContent);
                await HandleResponse(response);

                _logger.LogInformation(
                    "{Timestamp} {ServiceName} {OperationId}:: sending data to {Url} succeeded",
                    DateTime.Now, _serviceName, _operationId, url);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Timestamp} {ServiceName} {OperationId}:: Error sending request to {Url}",
                    DateTime.Now, _serviceName, _operationId, url);
                throw;
            }
        }

        public async Task PostAsync(string clientName, string url)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            try
            {
                PatchIfHeatHarmony(clientName, httpClient);

                _logger.LogInformation(
                    "{Timestamp} {ServiceName} {OperationId}:: sending POST to {Url} without body",
                    DateTime.Now, _serviceName, _operationId, url);

                using var response = await httpClient.PostAsync(url, null);
                await HandleResponse(response);

                _logger.LogInformation(
                    "{Timestamp} {ServiceName} {OperationId}:: sending data to {Url} succeeded",
                    DateTime.Now, _serviceName, _operationId, url);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Timestamp} {ServiceName} {OperationId}:: Error sending request to {Url}",
                    DateTime.Now, _serviceName, _operationId, url);
                throw;
            }
        }

        public async Task<TResult?> PostAsync<TResult>(string clientName, string url)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            try
            {
                PatchIfHeatHarmony(clientName, httpClient);

                _logger.LogInformation(
                    "{Timestamp} {ServiceName} {OperationId}:: sending POST to {Url} without body expecting {ResultType}",
                    DateTime.Now, _serviceName, _operationId, url, typeof(TResult).Name);

                using var response = await httpClient.PostAsync(url, null);
                await HandleResponse(response);

                var result = await ReadFromJsonASync<TResult>(response.Content);

                _logger.LogInformation(
                    "{Timestamp} {ServiceName} {OperationId}:: sending data to {Url} succeeded",
                    DateTime.Now, _serviceName, _operationId, url);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Timestamp} {ServiceName} {OperationId}:: Error sending request to {Url}",
                    DateTime.Now, _serviceName, _operationId, url);
                throw;
            }
        }

        public async Task<TResult?> PostAsync<TRequest, TResult>(string clientName, string url, TRequest data)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            try
            {
                PatchIfHeatHarmony(clientName, httpClient);
                var requestContent = SerializeToJson(data);
                using var response = await httpClient.PostAsync(url, requestContent);
                await HandleResponse(response);
                return await ReadFromJsonASync<TResult>(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Timestamp} {ServiceName} {OperationId}:: Error sending request to {Url}",
                    DateTime.Now, _serviceName, _operationId, url);
                throw;
            }
        }

        public async Task<(int StatusCode, string? Content)> PostRawAsync<TRequest>(string clientName, string url, TRequest data)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            try
            {
                PatchIfHeatHarmony(clientName, httpClient);
                var requestContent = SerializeToJson(data);

                using var response = await httpClient.PostAsync(url, requestContent);
                var content = await response.Content.ReadAsStringAsync();
                return ((int)response.StatusCode, string.IsNullOrWhiteSpace(content) ? null : content);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Timestamp} {ServiceName} {OperationId}:: Error sending raw POST request to {Url}",
                    DateTime.Now, _serviceName, _operationId, url);
                throw;
            }
        }


        public async Task<TResult?> GetAsync<TResult>(string clientName, string url)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            try
            {
                PatchIfHeatHarmony(clientName, httpClient);

                using var response = await httpClient.GetAsync(url);
                await HandleResponse(response);

                var result = await ReadFromJsonASync<TResult>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Timestamp} {ServiceName} {OperationId}:: Error getting from {Url}",
                    DateTime.Now, _serviceName, _operationId, url);
                throw;
            }
        }

        public async Task<string> GetStringAsync(string clientName, string url)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            try
            {
                PatchIfHeatHarmony(clientName, httpClient);

                using var response = await httpClient.GetAsync(url);
                await HandleResponse(response);

                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Timestamp} {ServiceName} {OperationId}:: Error getting from {Url}",
                    DateTime.Now, _serviceName, _operationId, url);
                throw;
            }
        }

        public async Task<TResult?> DeleteAsync<TResult>(string clientName, string url)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);
            try
            {
                using var response = await httpClient.DeleteAsync(url);
                await HandleResponse(response);

                _logger.LogInformation(
                    "{Timestamp} {ServiceName} {OperationId}:: deleting data from {Url} succeeded",
                    DateTime.Now, _serviceName, _operationId, url);
                return await ReadFromJsonASync<TResult>(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Timestamp} {ServiceName} {OperationId}:: Error deleting from {Url}",
                    DateTime.Now, _serviceName, _operationId, url);
                throw;
            }
        }

        private static async Task HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"{DateTime.Now} Request failed {response.StatusCode} with content {content}");
        }

        private static async Task<T?> ReadFromJsonASync<T>(HttpContent content)
        {
            using var contentStream = await content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<T>(contentStream);
            return data;
        }

        private static JsonContent SerializeToJson<T>(T data)
        {
            return JsonContent.Create(data);
        }

        private static void PatchIfHeatHarmony(string clientName, HttpClient httpClient)
        {
            if (clientName == HttpClientConst.HeatHarmony)
            {
                PatchHeatHarmonyClient(httpClient);
            }
        }
        private static void PatchHeatHarmonyClient(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add(GlobalConst.HeatHarmonyApiKeyHeaderName, GlobalConfig.HeatHarmonyConfig!.ApiKey);
        }
    }
}
