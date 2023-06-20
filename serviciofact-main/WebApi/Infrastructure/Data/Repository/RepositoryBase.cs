using System;
using System.Collections.Generic;
using WebApi.Application.Interface;
using WebApi.Domain.Entity;
using WebApi.Infrastructure.Data.Context;

namespace WebApi.Infrastructure.Data.Repository
{
    public class RepositoryBase<TEntity> : IRepository<InvoiceFactoring> where TEntity : InvoiceFactoring
    {
        protected readonly EmisionDbContext _contextE21;
        protected readonly FactoringDbContext _context;

        public RepositoryBase(FactoringDbContext context)
        {
            _context = context;
        }

        public RepositoryBase(EmisionDbContext context)
        {
            _contextE21 = context;
        }

        public void Add(InvoiceFactoring entity)
        {
            //Mapeo de Invoice a InvoiceFActoringTable
            InvoiceFactoringTable invoiceFactoringTable = new InvoiceFactoringTable
            {
                id_enterprise = entity.EnterpriseId,
                invoice_id = entity.InvoiceId,
                document_id = entity.DocumentId,
                invoice_issuedate = entity.InvoiceIssuedate,
                invoice_uuid = entity.InvoiceUuid,
                invoice_uuid_type = entity.InvoiceUuidType,
                customer_type_identification = entity.CustomerTypeIdentification,
                customer_identification = entity.CustomerIdentification,
                supplier_type_identification = entity.SupplierTypeIdentification,
                supplier_identification = entity.SupplierIdentification,
                path_file_xml = entity.PathFileXml,
                payable_amount = entity.PayableAmount,
                payment_date = entity.PaymentDate
            };

            _context.SaveInvoiceFactoring(invoiceFactoringTable);

        }

        public List<InvoiceFactoring> ConsultAnys()
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public InvoiceFactoring GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(string id, InvoiceFactoring entity)
        {
            throw new NotImplementedException();
        }
    }
}