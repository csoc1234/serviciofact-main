using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using Contributors.Infraestructure.Data.Context.Interface;
using Contributors.Models.Response;
using Contributors.Domain.Entities;

namespace Contributors.Models
{
    public partial class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        #region Atributos Globales

        private readonly string _connectionString;

        #endregion

        #region Constructores
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {

        }
        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Propiedades
        public DbSet<EnterpriseTable> Enterprise_Factoring { get; set; }

        public DbSet<Taxpayers> Taxpayers { get; set; }

        public DbSet<EnterpriseFactoringTask> EnterpriseFactoringTask { get; set; }

        public DbSet<EnterpriseFactoringHab> Enterprise_Factoring_Hab { get; set; }

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
            //modelBuilder.Entity<EnterpriseTable>(entity =>
            //{
            //  entity.Property(e => e.id).HasColumnName("id");

            //  entity.Property(e => e.id_enterprise)
            //.IsRequired()
            //.HasMaxLength(20);

            //  entity.Property(e => e.company_id)
            //.IsRequired()
            //.HasMaxLength(20);

            //  entity.Property(e => e.registration_name)
            //.IsRequired()
            //.HasMaxLength(20);

            //  entity.Property(e => e.verification_digit)
            //.IsRequired();

            //  entity.ToTable("enterprise_factoring");
            //});

            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #endregion

        #region Implementaciones
        public bool ChangeStatusIssuers(int idIssuer, StatusTask process)
        {
            try
            {
                SqlParameter IdIssuerParam = new SqlParameter
                {
                    ParameterName = "@IdIssuer",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = idIssuer
                };

                SqlParameter processParam = new SqlParameter
                {
                    ParameterName = "@Process",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = process.Process
                };

                SqlParameter processNameParam = new SqlParameter
                {
                    ParameterName = "@ProcessName",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = process.ProcessName.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = process.ProcessName
                };

                SqlParameter statusParam = new SqlParameter
                {
                    ParameterName = "@Status",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = process.Status
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                    IdIssuerParam,
                    processParam,
                    processNameParam,
                    statusParam
                };

                //Llamamos al SP y le pasamos los parametros

                var result = this.EnterpriseFactoringTask
                             .FromSqlRaw("EXEC dbo.uspEnterpriseTaskChangeStatus " +
                             "@IdIssuer, " +
                             "@Process, " +
                             "@ProcessName, " +
                             "@Status;",
                                      parameters).AsEnumerable().FirstOrDefault();

                return result != null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateTaxpayerStatusFactoring(string nit, int status, int environment)
        {
            try
            {
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                SqlParameter nitParam = new SqlParameter
                {
                    ParameterName = "@nit",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = nit
                };

                SqlParameter statusParam = new SqlParameter
                {
                    ParameterName = "@status",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = status
                };

                SqlParameter environmentParam = new SqlParameter
                {
                    ParameterName = "@environment",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = environment
                };

                SqlParameter[] parameters = new SqlParameter[]
                {idParam, nitParam, statusParam, environmentParam};

                return this.Database.ExecuteSqlRaw("EXEC dbo.uspUpdateStatusFactoring " +
                             "@id out, " +
                             "@nit, " +
                             "@status, " +
                             "@environment;", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateTaxpayerStatusHab(string nit, int status, int environment)
        {
            try
            {
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                SqlParameter nitParam = new SqlParameter
                {
                    ParameterName = "@nit",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = nit
                };

                SqlParameter statusParam = new SqlParameter
                {
                    ParameterName = "@status",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = status
                };

                SqlParameter environmentParam = new SqlParameter
                {
                    ParameterName = "@environment",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = environment
                };

                SqlParameter[] parameters = new SqlParameter[]
                {idParam, statusParam, nitParam, environmentParam};

                return this.Database.ExecuteSqlRaw("EXEC dbo.uspUpdateStatusHab " +
                             "@id out, " +
                             "@status, " +
                             "@nit, " +
                             "@environment;", parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase RegisterTaxpayerForEnableDian(string companyId, string testSetId)
        {
            try
            {
                SqlParameter companyIdParam = new SqlParameter
                {
                    ParameterName = "@companyId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = companyId.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = companyId
                };

                SqlParameter testSetIdParam = new SqlParameter
                {
                    ParameterName = "@testSetId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = testSetId.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = testSetId
                };

                //Parameter Output
                SqlParameter id = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                    companyIdParam,
                    testSetIdParam,
                    id
                };

                var result = this.Database.ExecuteSqlRaw("EXEC dbo.uspEnterpriseEnableDian @companyId, @testSetId, @id out;", parameters);

                return new ResponseBase { Code = (int)id.Value };
            }
            catch (Exception ex)
            {
                return new ResponseBase { Code = 500, Message = ex.Message };
            }
        }

        public List<Taxpayers> TaxPayerListEnableDian(int status)
        {
            try
            {
                SqlParameter statusParam = new SqlParameter
                {
                    ParameterName = "@statusValue",
                    SqlDbType = System.Data.SqlDbType.Int,                    
                    Direction = System.Data.ParameterDirection.Input,
                    Value = status
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                    statusParam
                };

                var result = this.Taxpayers.FromSqlRaw("EXEC dbo.uspGetTaxpayersStatus @statusValue;", parameters).ToList();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion
    }
}