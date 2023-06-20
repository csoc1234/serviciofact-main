using APIGetValidDocs.Infraestructure.Database;

namespace APIGetValidDocs.Domain.Interface
{
    public interface IThreadCreator
    {
        void GetFreeThread(List<TInvoiceFactoring> listInvoice);
    }
}
