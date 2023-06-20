namespace FeCoEventos.Models.Responses
{
    public class ResponseHttp<T> : ResponseBase
    {
        public T Result { get; set; }
    }
}
