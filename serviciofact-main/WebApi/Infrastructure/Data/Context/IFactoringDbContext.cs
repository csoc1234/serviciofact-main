using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Domain.Entity;
using TFHKA.LogsMongo;

namespace WebApi.Infrastructure.Data.Context
{
    public interface IFactoringDbContext
    {
        Task<List<InvoiceStatusLast>> GetInvoiceStatusLastAsync(string documentId, string identification, string uuid, ILogMongo log);

        Task<bool> InvoiceStatusEventHistUpdateAsync(int id, DateTime updateAt, ILogMongo log);

        Task<bool> InvoiceLastStatusCreateAsync(int id,  InvoiceStatusLast statusEntity, ILogMongo log);

    }
}
