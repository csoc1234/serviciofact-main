namespace APIGetValidDocs.Infraestructure.Interface
{
    public interface IBaseHttpClient
    {
        Task<T> Get<T>(string client, string api);

        Task<T> Post<T>(string client, string api, object body);
    }
}
