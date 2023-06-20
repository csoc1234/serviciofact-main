using APIGetValidDocs.Domain.Entity;

namespace APIGetValidDocs.Domain.Interface
{
    public interface IGetInvoiceDomain
    {
        Task<List<Invoice>> GetList(string typeIdentificationSupplier, string numberIdentificationSupplier, string typeIdentificationCustomer, string numberIdentificationCustomer, DateTime dateFrom, DateTime dateTo);
    }
}
