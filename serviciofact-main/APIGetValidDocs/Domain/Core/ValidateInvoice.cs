using APIGetValidDocs.Domain.Entity;
using APIGetValidDocs.Domain.Interface;
using APIGetValidDocs.Infraestructure.AzureStorage;
using APIGetValidDocs.Infraestructure.Database;
using APIGetValidDocs.Infraestructure.Interface;
using APIGetValidDocs.Infraestructure.SiteRemote;
using TFHKA.Storage.Fileshare.Client.Interface;
using TFHKA.Storage.Fileshare.Client.Models;

namespace APIGetValidDocs.Domain.Core
{
    public class ValidateInvoice
    {
        private readonly IConfiguration _configuration;

        private readonly IBaseHttpClient _baseHttpClient;

        private readonly IInvoiceClient _invoiceClient;

        private readonly IStorageFiles _storageFiles;

        private readonly IValidDocsDbContext _validDocsDbContext;

        private readonly IFileShareClass _fileShareClass;

        private const string _nameFileAttached = "_AttachmentDocument.xml";

        private readonly TInvoiceFactoring _invoice;

        private IAccumulatorsMetric _accumulators;

        private readonly int _hilo;

        public ValidateInvoice(
            IConfiguration configuration,
            TInvoiceFactoring invoice,
            int hilo,
            IBaseHttpClient baseHttpClient,
            IFileShareClass fileShareClass,
            IAccumulatorsMetric accumulators)
        {
            _baseHttpClient = baseHttpClient;
            _fileShareClass = fileShareClass;
            _configuration = configuration;
            _invoiceClient = new InvoiceClient(_baseHttpClient, _configuration);
            _storageFiles = new StorageFiles(_configuration, _fileShareClass);
            _validDocsDbContext = new ValidDocsDbContext(_configuration);
            _invoice = invoice;
            _accumulators = accumulators;
            _hilo = hilo;
        }

        public async void Validate()
        {
            //Get Xml from Storage
            StorageFileResponse fileXml = _storageFiles.GetFile(_invoice.PathFileXML, _invoice.DocumentId + _nameFileAttached, StorageConfiguration.EmisionFileShare);

            if (fileXml.Code == 200)
            {
                //Valid events from Invoice 
                InvoiceValidate invoiceValidate = new InvoiceValidate
                {
                    DocumentId = _invoice.DocumentId,
                    Cufe = _invoice.InvoiceUuid,
                    Xml = fileXml.File,
                    IdEnterprise = _invoice.IdEnterprise,
                    TypeIdentificationSupplier = _invoice.SupplierTypeIdentification,
                    NumberIdentificationSupplier = _invoice.SupplierIdentification,
                    DatePayment = null
                };

                InvoiceStatus resultValidate = await _invoiceClient.Post(invoiceValidate);
                if (resultValidate != null)
                {
                    Invoice invoice = new Invoice
                    {
                        DocumentId = _invoice.DocumentId,
                        Cufe = _invoice.InvoiceUuid,
                        Xml = fileXml.File,
                        IssueDate = _invoice.InvoiceIssuedate,
                        PayableAmount = _invoice.PayableAmount,
                        PaymentDate = _invoice.PaymentDate,
                        EventCode = resultValidate.EventCode
                    };

                    if (resultValidate.Valid)
                    {
                        //Accumulator
                        _accumulators.AddResult(invoice);
                    }
                    else
                    {
                        if (resultValidate.EventCode != null)
                        {
                            //Disable invoice invalid and remove from list

                            //Verifico si tiene un reclamo para desactivarla
                            if (resultValidate.EventCode.Where(x => x == "031").Count() > 0)
                            {
                                _validDocsDbContext.UpdateInvoiceFactoring(_invoice.Id, 31, _configuration);
                            }
                        }
                    }
                }
                else
                {

                }
            }
            else
            {
                //Update Invoice Disable - Xml Not Found
                _validDocsDbContext.UpdateInvoiceFactoring(_invoice.Id, 104, _configuration);
            }
        }
    }
}
