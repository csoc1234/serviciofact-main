using APIFactoringIntegration.Domain.Entity;
using APIFactoringIntegration.Infraestructure.Interface;

namespace APIFactoringIntegration.Infraestructure.SiteRemote
{
    public class APIGetValidDocs : IAPIGetValidDocs
    {
        private readonly IBaseHttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public APIGetValidDocs(IBaseHttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Invoice>> Post(SearchFilter request)
        {
            try
            {
                List<Invoice> result = await _httpClient.Post<List<Invoice>>(
                _configuration["Endpoint:APIGetValidDocs"],
                _configuration["Endpoint:APIGetValidDocsApi"],
                request);

                return result ?? new List<Invoice>();
            }
            catch (Exception ex)
            {
                return new List<Invoice>();
            }
        }
    }
}
