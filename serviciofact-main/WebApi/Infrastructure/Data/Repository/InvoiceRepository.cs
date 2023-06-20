using WebApi.Domain.Core;
using WebApi.Application.Interface;
using WebApi.Infrastructure.Data.Context;
using WebApi.Domain.Entity;
using System.Collections.Generic;
using System;

namespace WebApi.Infrastructure.Data.Repository
{
  public class InvoiceRepository : RepositoryBase<InvoiceFactoring>, IInvoiceRepository
  {
    public InvoiceRepository(EmisionDbContext contextE21)
      :base(contextE21)
    { }

    public InvoiceRepository(FactoringDbContext context)
      : base(context)
    { }
    public Invoice21Domain GetEmi21ById(int id)
    {
      Invoice21Table result = _contextE21.GetAnysInvoiceAsync(id);
      Invoice21Domain invoice = null;

      if (result != null)
      {
        invoice = new Invoice21Domain
        {
          Invoice = new Invoice
          {
            Id = result.Id,
            IdEnterprise = result.IdEnterprise,
            DocumentId = result.DocumentId,
            PathFile = result.PathFile,
            NameFile = result.NameFile,
            Cufe = result.Uuid
          }
        };
      }

      return invoice;
    }

    public int ValidateInvoiceWhitReferenceDoc(int invoiceId)
    {
      var result = _context.TableInvoiceExistsAsync(invoiceId).Result;
      
      return result;
    }

    public List<Invoice21Domain> GetInvoicesPerTaxpayer(int idEnterprice, DateTime dateIni, DateTime dateEnd)
    {
      var listInvoices = new List<Invoice21Domain>();

      var invoicesPerTaxpayerList = _contextE21.GetInvoicesPerTaxpayer(idEnterprice, dateIni, dateEnd);

      foreach(Invoice21Table item in invoicesPerTaxpayerList.Result)
      {
        var invoice = new Invoice {
          Id = item.Id,
          DocumentId = item.DocumentId,
          Cufe = item.Uuid,
          IdEnterprise = idEnterprice,
          PathFile = item.PathFile,
          NameFile = item.NameFile,
          TrackId = item.TrackId
        };

        var invoiceDomain = new Invoice21Domain { 
          Invoice = invoice
        };

        listInvoices.Add(invoiceDomain);
      }

      return listInvoices;
    }

    public List<Invoice21Domain> GetInvoicesInHab(string nit)
    {
      var listInvoices = new List<Invoice21Domain>();

      var invoicesPerTaxpayerHabList = _contextE21.GetInvoicesHab(nit);

      foreach (Invoice21Table item in invoicesPerTaxpayerHabList.Result)
      {
        var invoice = new Invoice
        {
          Id = item.Id,
          DocumentId = item.DocumentId,
          Cufe = item.Uuid,
          IdEnterprise = item.IdEnterprise,
          PathFile = item.PathFile,
          NameFile = item.NameFile,
          TrackId = item.TrackId
        };

        var invoiceDomain = new Invoice21Domain
        {
          Invoice = invoice
        };

        listInvoices.Add(invoiceDomain);
      }

      return listInvoices;
    }

    List<Invoice21Domain> IRepository<Invoice21Domain>.ConsultAnys()
    {
      throw new NotImplementedException();
    }

    Invoice21Domain IRepository<Invoice21Domain>.GetById(int id)
    {
      throw new NotImplementedException();
    }

    public void Add(Invoice21Domain entity)
    {
      throw new NotImplementedException();
    }

    public void Update(string id, Invoice21Domain entity)
    {
      throw new NotImplementedException();
    }
  }
}