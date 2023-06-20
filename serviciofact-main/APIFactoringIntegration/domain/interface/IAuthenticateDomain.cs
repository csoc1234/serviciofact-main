namespace APIFactoringIntegration.Domain.Interface
{
    public interface IAuthenticateDomain
    {
        Task<bool> Validate(string user, string password);
    }
}
