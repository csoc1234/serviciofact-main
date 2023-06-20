using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Application.Dto;
using TFHKA.LogsMongo;
using WebApi.Models.Response;

namespace WebApi.Application.Interface
{
   public interface IGestionInvoice
    {
        ResponseDto GetInfoInvoice(InvoiceDto invoiceDto, ILogMongo log);
        InvoicesPerTaxpayerResponse GetInvoicesPerTaxpayerList(InvoicesPerTaxpayerDto invoicesPerTaxpayerDto, ILogMongo log);
        ResponseDto AddInfoInvoice(InvoiceDto invoiceDto, ILogMongo log);
        InvoicesPerTaxpayerResponse GetInvoicesInHabilitation(string nit, ILogMongo log);
  }
}
