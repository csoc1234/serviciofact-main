using APIGetValidDocs.Infraestructure.Database;

namespace APIGetValidDocs.Infraestructure.Interface
{
    public interface IValidDocsDbContext
    {
        public List<TInvoiceFactoring> ReadValidDocsForFactoring(Int16 limit, string supplierTypeIdentification, string supplierIdentification, string customerTypeIdentification, string customerIdentification, DateTime dateFrom, DateTime dateTo, IConfiguration configuration);

        public int UpdateInvoiceFactoring(int id, Int16 status, IConfiguration configuration);
    }
}
