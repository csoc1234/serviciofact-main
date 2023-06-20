using APIGetValidDocs.Domain.Entity;
using APIGetValidDocs.Domain.Interface;
using APIGetValidDocs.Infraestructure.Database;
using APIGetValidDocs.Infraestructure.Interface;
using TFHKA.Storage.Fileshare.Client.Interface;

namespace APIGetValidDocs.Domain.Core
{
    public class GetInvoiceDomain : IGetInvoiceDomain
    {
        private readonly IInvoiceClient _invoiceClient;

        private readonly IStorageFiles _storageFiles;

        private readonly IValidDocsDbContext _validDocsDbContext;

        private readonly IConfiguration _configuration;

        private const string _nameFileAttached = "_AttachmentDocument.xml";

        private IAccumulatorsMetric _accumulators;

        private readonly IBaseHttpClient _baseHttpClient;

        private readonly IFileShareClass _fileShareClass;

        public GetInvoiceDomain(IInvoiceClient invoiceClient, IStorageFiles storageFiles, IValidDocsDbContext validDocsDbContext, IConfiguration configuration, IAccumulatorsMetric accumulators, IBaseHttpClient baseHttpClient, IFileShareClass fileShareClass)
        {
            _invoiceClient = invoiceClient;
            _storageFiles = storageFiles;
            _validDocsDbContext = validDocsDbContext;
            _configuration = configuration;
            _accumulators = accumulators;
            _baseHttpClient = baseHttpClient;
            _fileShareClass = fileShareClass;
        }

        public async Task<List<Invoice>> GetList(string typeIdentificationSupplier, string numberIdentificationSupplier, string typeIdentificationCustomer, string numberIdentificationCustomer, DateTime dateFrom, DateTime dateTo)
        {
            List<Invoice> listValid = new List<Invoice>();

            try
            {
                //Consuming SP
                short limit = short.Parse(_configuration["LimitQuery"]);
                List<TInvoiceFactoring> list = _validDocsDbContext.ReadValidDocsForFactoring(
                    limit,
                    typeIdentificationSupplier,
                    numberIdentificationSupplier,
                    typeIdentificationCustomer,
                    numberIdentificationCustomer,
                    dateFrom,
                    dateTo,
                    _configuration
                    );

                if (list != null)
                {
                    //MultiHilo
                    ThreadCreator creator = new ThreadCreator(_configuration, _accumulators, _baseHttpClient, _fileShareClass);
                    creator.GetFreeThread(list);

                    //Cuales son los Exitosos
                    List<Invoice> resultValid = _accumulators.GetResult();

                    //Limpio el Accumulator
                    _accumulators.Clean();

                    //Return
                    return resultValid;
                }
                else
                {
                    return listValid;
                }
            }
            catch (Exception ex)
            {
                return listValid;
            }
        }
    }
}
