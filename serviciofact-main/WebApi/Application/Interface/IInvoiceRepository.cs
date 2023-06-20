using WebApi.Domain.Core;
using System;
using System.Collections.Generic;

namespace WebApi.Application.Interface
{
  public interface IInvoiceRepository : IRepository<Invoice21Domain>
  {
    int ValidateInvoiceWhitReferenceDoc(int invoiceId);
    Invoice21Domain GetEmi21ById(int id);
    List<Invoice21Domain> GetInvoicesPerTaxpayer(int idEnterprice, DateTime dateIni, DateTime dateEnd);
    List<Invoice21Domain> GetInvoicesInHab(string nit);
  }
}