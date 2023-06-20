using Contributors.Application.Dto.Response;
using Contributors.Infraestructure.Logging;

namespace Contributors.Application.Interface
{
    public interface ITaxpayerListStatus
    {
        TaxPayerListStatusResponse GetList(int status, LogRequest logRequest);
    }
}
