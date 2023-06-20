using APIFactoringIntegration.Domain.Entity;

namespace APIFactoringIntegration.Infraestructure.Interface
{
    public interface ICredentialsDbContext
    {
        Task<bool> ValidateCredentials(CredentialsManagement credentials, IConfiguration configuration);
    }
}
