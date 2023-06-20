using APIGetValidDocs.Infraestructure.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace APIGetValidDocs.Infraestructure.Database
{
    public class ValidDocsDbContext : DbContext, IValidDocsDbContext
    {
        private readonly IConfiguration _configuration;

        public ValidDocsDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<TInvoiceFactoring> Invoices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:DefaultConnection"]);
            }
        }

        public List<TInvoiceFactoring> ReadValidDocsForFactoring(Int16 limit, string supplierTypeIdentification, string supplierIdentification, string customerTypeIdentification, string customerIdentification, DateTime dateFrom, DateTime dateTo, IConfiguration configuration)
        {
            try
            {
                SqlParameter limitParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@limit",
                    SqlDbType = System.Data.SqlDbType.SmallInt,
                    Value = limit
                };

                SqlParameter supplierTypeIdentificationParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@supplierTypeIdentification",
                    //Size = 2,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = supplierTypeIdentification
                };

                SqlParameter supplierIdentificationParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@supplierIdentification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = supplierIdentification
                };

                SqlParameter customerTypeIdentificationParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@customerTypeIdentification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = customerTypeIdentification
                };

                SqlParameter customerIdentificationParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@customerIdentification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Value = customerIdentification
                };

                SqlParameter dateFromParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@dateFrom",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Value = dateFrom
                };
                
                SqlParameter dateToParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@dateTo",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Value = dateTo
                };

                SqlParameter[] parameters = new SqlParameter[]
                {
                    limitParam,
                    supplierTypeIdentificationParam,
                    supplierIdentificationParam,
                    customerTypeIdentificationParam,
                    customerIdentificationParam,
                    dateFromParam,
                    dateToParam
                };

                using (ValidDocsDbContext? dbo = new ValidDocsDbContext(configuration))
                {
                    List<TInvoiceFactoring>? result = dbo.Invoices.FromSqlRaw("EXEC uspGetValidDocs " +
                           "@limit, " + "@supplierTypeIdentification, " + "@supplierIdentification, " +
                           "@customerTypeIdentification, " + "@customerIdentification, " + "@dateFrom, " + "@dateTo; ", parameters)
                        .ToListAsync().Result;

                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int UpdateInvoiceFactoring(int id, Int16 status, IConfiguration configuration)
        {
            try
            {
                SqlParameter idParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = id
                };

                SqlParameter statusParam = new SqlParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    ParameterName = "@status",
                    SqlDbType = System.Data.SqlDbType.SmallInt,
                    Value = status
                };

                SqlParameter[] parameters = new SqlParameter[]
                {
                    idParam,
                    statusParam
                };

                using (ValidDocsDbContext? dbo = new ValidDocsDbContext(configuration))
                {
                    int result = dbo.Database.ExecuteSqlRaw("EXEC uspDisableInvoice " +
                           "@id, " + "@status; ", parameters);

                    return result;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

    }
}
