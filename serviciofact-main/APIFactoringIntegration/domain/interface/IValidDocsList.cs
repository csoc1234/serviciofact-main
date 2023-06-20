using APIFactoringIntegration.Domain.Entity;

namespace APIFactoringIntegration.Domain.Interface
{
    public interface IValidDocsList
    {
        Task<List<Invoice>> Get(string usuario, string contraseña, string tipoDocumentoProveedor, string idProveedor, string tipoDocumentoPagador, string idPagador, string fechaInicio, string FechaFin);
    }
}
