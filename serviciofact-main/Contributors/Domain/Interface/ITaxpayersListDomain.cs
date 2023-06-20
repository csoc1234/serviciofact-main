using Contributors.Application.Dto.Response;

namespace Contributors.Domain.Interface
{
    public interface ITaxpayersListDomain
    {
        TaxPayerListStatusResponse GetList(int status);
    }
}
