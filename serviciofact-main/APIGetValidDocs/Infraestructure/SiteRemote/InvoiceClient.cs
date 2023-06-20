using APIGetValidDocs.Domain.Entity;
using APIGetValidDocs.Infraestructure.Interface;

namespace APIGetValidDocs.Infraestructure.SiteRemote
{
    public class InvoiceClient : IInvoiceClient
    {
        private readonly IBaseHttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public InvoiceClient(IBaseHttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<InvoiceStatus> Post(InvoiceValidate request)
        {
            try
            {
                InvoiceStatus result = await _httpClient.Post<InvoiceStatus>(
                _configuration["Endpoint:ValidateEvents"],
                _configuration["Endpoint:ValidateEventsApi"],
                request);

                return result ?? null;
            }
            catch (Exception ex)
            {
                return new InvoiceStatus
                {
                    Code = 500,
                    Message = "Error al momento de procesar la transaccion. " + ex.Message
                };
            }
        }
    }
}
