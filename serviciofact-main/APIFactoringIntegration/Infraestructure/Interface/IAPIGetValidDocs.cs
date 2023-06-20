using APIFactoringIntegration.Domain.Entity;

namespace APIFactoringIntegration.Infraestructure.Interface
{
    public interface IAPIGetValidDocs
    {
        Task<List<Invoice>> Post(SearchFilter request);
    }
}
