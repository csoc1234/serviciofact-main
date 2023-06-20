using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApi.Infrastructure.Data.Context
{
    /// <summary>
    /// Clase Context para manejo de DB
    /// </summary>
    public partial class EmisionDbContext : DbContext
    {
        #region Atributos Globales

        private readonly string _connectionString;

        #endregion

        #region Constructores

        public EmisionDbContext(DbContextOptions<EmisionDbContext> options)
          : base(options)
        {

        }

        public EmisionDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Propiedades
        public DbSet<Invoice21Table> Invoice { get; set; }
        
        #endregion

        #region Sobrecargas
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            { //Para uso de Migrations y Testing              
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Para uso de Migrations
            modelBuilder.Entity<Invoice21Table>(entity =>
            {
                //entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.Id);

                entity.Property(e => e.DocumentId)
              .IsRequired()
              .HasMaxLength(20);

                entity.ToTable("invoice21");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #endregion

        #region Implementaciones
        /// <summary>
        /// Metodo de implementacion para consumumir el SP Si existe la factura no procesa y si no existe la factura procede
        /// </summary>
        /// <param name="idInvoice">Identificador Primario de la Factura en Emision21</param>
        /// <returns></returns>
        public Invoice21Table GetAnysInvoiceAsync(int idInvoice)
        {
            try
            {
                // Settings Parameters Input.
                SqlParameter idInvoiceParam = new SqlParameter
                {
                    ParameterName = "@idInvoice",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = idInvoice
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                idInvoiceParam,
                };

                //Llamamos al SP y le pasamos los parametros

                return this.Invoice
                           .FromSqlRaw("EXEC dbo.GetInvoiceData " +
                           "@idInvoice;",
                           parameters)
                           .AsEnumerable()
                           .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<List<Invoice21Table>> GetInvoicesPerTaxpayer(int idEnterprise, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                SqlParameter idEnterpriseParam = new SqlParameter
                {
                    ParameterName = "@idEnterprise",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = idEnterprise
                };

                SqlParameter dateFromParam = new SqlParameter
                {
                    ParameterName = "@dateFrom",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = dateFrom
                };

                SqlParameter dateToParam = new SqlParameter
                {
                    ParameterName = "@dateTo",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = dateTo
                };

                SqlParameter[] parameters = new SqlParameter[]
                {
                idEnterpriseParam, dateFromParam, dateToParam
                };

                return this.Invoice
                           .FromSqlRaw("EXEC dbo.GetListInvoiceForFactoring " +
                           "@idEnterprise, " +
                           "@dateFrom, " +
                           "@dateTo; ",
                           parameters)
                           .ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<List<Invoice21Table>> GetInvoicesHab(string nit)
        {
          try
          {
            SqlParameter nitParam = new SqlParameter
            {
              ParameterName = "@nitContribuyente",
              SqlDbType = System.Data.SqlDbType.VarChar,
              Direction = System.Data.ParameterDirection.Input,
              Value = nit
            };      

            SqlParameter[] parameters = new SqlParameter[]
            {
                nitParam
            };

            return this.Invoice
                       .FromSqlRaw("EXEC dbo.GetListInvoicesHab " +
                       "@nitContribuyente; ",
                       parameters)
                       .ToListAsync();
          }
          catch (Exception ex)
          {
            throw ex;
          }
        }

        #endregion
  }
}