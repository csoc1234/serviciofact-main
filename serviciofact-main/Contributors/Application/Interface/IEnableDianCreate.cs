using Contributors.Application.Dto;
using Contributors.Infraestructure.Logging;
using Contributors.Models.Response;

namespace Contributors.Application.Interface
{
    public interface IEnableDianCreate
    {

        ResponseBase Register(TaxPayersDto request, LogRequest logRequest);
    }
}
