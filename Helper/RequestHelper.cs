using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class RequestHelper
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private HttpClient _httpClient;

        public RequestHelper(string SubscriptionKey, string baseUri)
        {
            _serializerSettings = new JsonSerializerSettings();
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            //    NullValueHandling = NullValueHandling.Ignore
            //};
            _serializerSettings.Converters.Add(new StringEnumConverter());
            _httpClient = CreateHttpClient(SubscriptionKey, baseUri);
        }

        public async Task<TResult> GetAsync<TResult>(string uri, string authenticationToken = null)
        {
            HandleAuthorization(authenticationToken);
            uri = _httpClient.BaseAddress + uri;
            var response = await _httpClient.GetAsync(uri);
            var responseData = await response.Content.ReadAsStringAsync();
            //SI !IsSuccessStatusCode lanza Exception
            HandleResponse(response.IsSuccessStatusCode, response.ReasonPhrase, response.StatusCode, responseData);

            return JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings);
        }

        public async Task<TResult> PostAsync<TResult, T>(string uri, T data, string authenticationToken = null)
        {
            HandleAuthorization(authenticationToken);
            uri = _httpClient.BaseAddress + uri;
            var serialized = JsonConvert.SerializeObject(data, _serializerSettings);
            var response = await _httpClient.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
            var responseData = await response.Content.ReadAsStringAsync();
            //SI !IsSuccessStatusCode lanza Exception
            HandleResponse(response.IsSuccessStatusCode, response.ReasonPhrase, response.StatusCode, responseData);

            return JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings);
        }

        public async Task<TResult> PostTextAsync<TResult, T>(string uri, string data, string authenticationToken = null)
        {
            HandleAuthorization(authenticationToken);
            uri = _httpClient.BaseAddress + uri;
            var response = await _httpClient.PostAsync(uri, new StringContent(data, Encoding.UTF8, "text/plain"));
            var responseData = await response.Content.ReadAsStringAsync();
            //SI !IsSuccessStatusCode lanza Exception
            HandleResponse(response.IsSuccessStatusCode, response.ReasonPhrase, response.StatusCode, responseData);

            return JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings);
        }

        public async Task<TResult> PostAsync<TResult>(string uri, string authenticationToken = null)
        {
            HandleAuthorization(authenticationToken);
            uri = _httpClient.BaseAddress + uri;
            var response = await _httpClient.PostAsync(uri, null);
            var responseData = await response.Content.ReadAsStringAsync();
            //SI !IsSuccessStatusCode lanza Exception
            HandleResponse(response.IsSuccessStatusCode, response.ReasonPhrase, response.StatusCode, responseData);

            return JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings);
        }

        public async Task<TResult> PutAsync<TResult, T>(string uri, T data, string authenticationToken = null)
        {
            HandleAuthorization(authenticationToken);
            uri = _httpClient.BaseAddress + uri;
            var serialized = JsonConvert.SerializeObject(data, _serializerSettings);
            var response = await _httpClient.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
            var responseData = await response.Content.ReadAsStringAsync();
            //SI !IsSuccessStatusCode lanza Exception
            HandleResponse(response.IsSuccessStatusCode, response.ReasonPhrase, response.StatusCode, responseData);

            return JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings);
        }

        public async Task<TResult> DeleteAsync<TResult>(string uri, string authenticationToken = null)
        {
            HandleAuthorization(authenticationToken);

            var response = await _httpClient.DeleteAsync(uri);
            var responseData = await response.Content.ReadAsStringAsync();
            //SI !IsSuccessStatusCode lanza Exception
            HandleResponse(response.IsSuccessStatusCode, response.ReasonPhrase, response.StatusCode, responseData);

            return JsonConvert.DeserializeObject<TResult>(responseData, _serializerSettings);
        }

        private HttpClient CreateHttpClient(string SubscriptionKey, string baseUri)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            httpClient.BaseAddress = new Uri(baseUri);
            return httpClient;
        }

        private void HandleResponse(bool isSuccessStatusCode, string reasonPhrase, HttpStatusCode statusCode, string responseData)
        {
            if (!isSuccessStatusCode)
            {
                var content = string.Empty;

                try
                {
                    var obj = JsonConvert.DeserializeObject<APIExceptionMessage>(responseData, _serializerSettings);

                    if (obj != null)
                    {
                        content = obj.message;
                    }
                    else
                    {
                        content = reasonPhrase;
                    }
                }
                catch (System.Exception)
                {
                    content = reasonPhrase;
                }

                if (statusCode == HttpStatusCode.Forbidden || statusCode == HttpStatusCode.Unauthorized)
                {
                    // throw new ServiceAuthenticationException(content);
                }

                throw new HttpRequestException(content);
            }
        }

        private void HandleAuthorization(string authenticationToken)
        {
            if (!string.IsNullOrEmpty(authenticationToken))
            {
                _httpClient.DefaultRequestHeaders.Add("token", authenticationToken);
            }
            //else
            //{
            //    _httpClient.DefaultRequestHeaders.Authorization = null;
            //}
        }
    }
}
