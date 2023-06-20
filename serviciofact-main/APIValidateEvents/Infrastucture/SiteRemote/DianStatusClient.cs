using APIValidateEvents.Domain.Entity;
using APIValidateEvents.Infrastucture.SiteRemote.Interface;
using System.Diagnostics;

namespace APIValidateEvents.Infrastucture.SiteRemote
{
    public class DianStatusClient : IDianStatusClient
    {
        private readonly IBaseHttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DianStatusClient(IBaseHttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<InvoiceStatusDian> Get(string cufe, string supplierIdentification, string documentId)
        {
            try
            {
                InvoiceStatusDian result = new() { };
                result = await _httpClient.Get<InvoiceStatusDian>(
                    _configuration["Endpoint:StatusDianUrl"],
                    _configuration["Endpoint:StatusDianApi"] + cufe + "/" + supplierIdentification + "/" + documentId);
                return result ?? null;
            }
            catch (Exception ex)
            {
                return new InvoiceStatusDian {
                    InvoiceStatusCode = 500,
                    InvoiceStatusDesc = "Error al momento de procesar la transaccion. " + ex.Message };
            }
        }
    }
}
