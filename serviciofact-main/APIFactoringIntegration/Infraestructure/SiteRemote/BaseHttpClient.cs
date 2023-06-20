using APIFactoringIntegration.Infraestructure.Interface;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace APIFactoringIntegration.Infraestructure.SiteRemote
{
    public class BaseHttpClient : IBaseHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> Post<T>(string client, string api, object body)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient(client);

                httpClient.BaseAddress = new Uri(client);

                StringContent httpContent = new(JsonConvert.SerializeObject(body), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

                HttpResponseMessage response = await httpClient.PostAsync(api, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    string httpResult = await response.Content.ReadAsStringAsync();
                    T result = JsonConvert.DeserializeObject<T>(httpResult);
                    return result;
                }
                else
                {
                    ErrorConnection(response.StatusCode);
                }
                return default(T);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public void ErrorConnection(HttpStatusCode httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case HttpStatusCode.NotFound:
                    break;
                case HttpStatusCode.Forbidden:
                    break;
                case HttpStatusCode.Unauthorized:
                    break;
                case HttpStatusCode.InternalServerError:
                    break;
                case HttpStatusCode.BadRequest:
                    break;
                default:
                    break;
            }
        }
    }
}
