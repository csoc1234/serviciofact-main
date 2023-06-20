using APIFactoringIntegration.Domain.Entity;
using APIFactoringIntegration.Domain.Interface;
using APIFactoringIntegration.Infraestructure.Interface;

namespace APIFactoringIntegration.Domain.Core
{
    public class ValidDocsList : IValidDocsList
    {

        private readonly IAPIGetValidDocs _apiGetValidDocs;

        private readonly IAuthenticateDomain _authenticate;
        public ValidDocsList(IAPIGetValidDocs apiGetValidDocs, IAuthenticateDomain authenticate)
        {
            _apiGetValidDocs = apiGetValidDocs;
            _authenticate = authenticate;
        }

        public async Task<List<Invoice>> Get(string usuario, string contraseña, string tipoDocumentoProveedor, string idProveedor, string tipoDocumentoPagador, string idPagador, string fechaInicio, string FechaFin)
        {
            List<Invoice> validList = new List<Invoice>();

            try
            {
                bool isValidUser = await _authenticate.Validate(usuario, contraseña);

                if (isValidUser)
                {
                    SearchFilter request = new SearchFilter()
                    {
                        SupplierIdentificationType = tipoDocumentoProveedor,
                        SupplierIdentification = idProveedor,
                        CustomerIdentificationType = tipoDocumentoPagador,
                        CustomerIdentification = idPagador,
                        DateFrom = fechaInicio,
                        DateTo = FechaFin
                    };
                    validList = await _apiGetValidDocs.Post(request);
                }
            }
            catch (Exception ex)
            {
            }
            return validList;
        }
    }
}
