namespace APIFactoringIntegration.Infraestructure.Interface
{
    public interface IBaseHttpClient
    {
        Task<T> Post<T>(string client, string api, object body);
    }
}
