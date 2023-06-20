using APIFactoringIntegration.Domain.Entity;
using APIFactoringIntegration.Domain.Interface;
using APIFactoringIntegration.Infraestructure.Interface;

namespace APIFactoringIntegration.Domain.Core
{
    public class AuthenticateDomain : IAuthenticateDomain
    {
        private readonly ICredentialsDbContext _credentialsDbContext;
        private readonly IConfiguration _configuration;

        public AuthenticateDomain(ICredentialsDbContext credentialsDbContext, IConfiguration configuration)
        {
            _credentialsDbContext = credentialsDbContext;
            _configuration = configuration;
        }

        public async Task<bool> Validate(string user, string password)
        {
            try
            {
                CredentialsManagement credentials = new CredentialsManagement
                {
                    User = user,
                    Password = password
                };

                bool result = await _credentialsDbContext.ValidateCredentials(credentials, _configuration);

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
