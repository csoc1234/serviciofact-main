using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApi.Domain.Entity;
using TFHKA.LogsMongo;

namespace WebApi.Infrastructure.Data.Context
{
    public partial class FactoringDbContext : DbContext, IFactoringDbContext
    {
        #region Atributos Globales

        private readonly string _connectionString;

        #endregion

        #region Constructores

        public FactoringDbContext(DbContextOptions<FactoringDbContext> options)
          : base(options)
        {

        }

        public FactoringDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Propiedades
        public DbSet<InvoiceFactoringTable> InvoiceFactoringRow { get; set; }

        public DbSet<InvoiceStatusLast> InvoiceStatusLast { get; set; }

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
            //modelBuilder.Entity<InvoiceFactoringTable>(entity =>
            //{
            //  entity.Property(e => e.Id).HasColumnName("id");

            //  entity.Property(e => e.DocumentId)
            //      .IsRequired()
            //      .HasMaxLength(20);

            //  entity.ToTable("InvoiceFactoring");
            //});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #endregion

        #region Implementaciones
        /// <summary>
        /// Metodo de implementacion para consumumir el SP [uspTableExists] que indica si una Factura tiene documentos referenciados o no
        /// </summary>
        /// <param name="invoiceId">Numero Documento Factura</param>
        /// <returns></returns>
        public async Task<int> TableInvoiceExistsAsync(int invoiceId)
        {
            try
            {
                //Settings parameters input
                SqlParameter invoiceIdParam = new SqlParameter
                {
                    ParameterName = "@invoiceId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceId
                };

                //Parameter Output
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
          invoiceIdParam, idParam
                };

                //Llamamos al SP y le pasamos los parametros
                await this.Database.ExecuteSqlRawAsync("EXEC uspTableExists " +
                           "@invoiceId," +
                           "@id out",
                            parameters);

                string strObjt = idParam.Value.ToString();

                if (strObjt == "")
                    return 0;
                else
                    return Convert.ToInt32(strObjt);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Metodo de implementacion de SP de Persistencia de Facturas Factoring
        /// </summary>
        /// <param name="invoiceFactoring">Entidad con los valores</param>
        /// <returns></returns>
        public int SaveInvoiceFactoring(InvoiceFactoringTable invoiceFactoring)
        {
            try
            {
                // Settings Parameters Input.         
                SqlParameter id_enterprise = new SqlParameter
                {
                    ParameterName = "@id_enterprise",
                    SqlDbType = System.Data.SqlDbType.Int,
                    // Size = invoiceFactoring.id_enterprise.ToString().Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.id_enterprise
                };

                SqlParameter invoice_id = new SqlParameter
                {
                    ParameterName = "@invoice_id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    // Size = invoiceFactoring.invoice_id.ToString().Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.invoice_id
                };

                SqlParameter document_id = new SqlParameter
                {
                    ParameterName = "@document_id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    // Size = invoiceFactoring.document_id.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.document_id
                };

                SqlParameter invoice_uuid = new SqlParameter
                {
                    ParameterName = "@invoice_uuid",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    //  Size = invoiceFactoring.invoice_uuid.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.invoice_uuid
                };

                SqlParameter invoice_uuid_type = new SqlParameter
                {
                    ParameterName = "@invoice_uuid_type",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    //  Size = invoiceFactoring.invoice_uuid_type.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.invoice_uuid_type
                };

                SqlParameter invoice_issuedate = new SqlParameter
                {
                    ParameterName = "@invoice_issuedate",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    //  Size = invoiceFactoring.invoice_issuedate.ToString().Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.invoice_issuedate
                };

                SqlParameter supplier_type_identification = new SqlParameter
                {
                    ParameterName = "@supplier_type_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    //  Size = invoiceFactoring.supplier_type_identification.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.supplier_type_identification
                };

                SqlParameter supplier_identification = new SqlParameter
                {
                    ParameterName = "@supplier_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    //  Size = invoiceFactoring.supplier_identification.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.supplier_identification
                };

                SqlParameter customer_type_identification = new SqlParameter
                {
                    ParameterName = "@customer_type_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    //  Size = invoiceFactoring.customer_type_identification.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.customer_type_identification
                };

                SqlParameter customer_identification = new SqlParameter
                {
                    ParameterName = "@customer_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    //  Size = invoiceFactoring.customer_identification.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.customer_identification
                };

                SqlParameter path_file_xml = new SqlParameter
                {
                    ParameterName = "@path_file_xml",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    //  Size = invoiceFactoring.path_file.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.path_file_xml
                };

                SqlParameter payment_date = new SqlParameter
                {
                    ParameterName = "@payment_date",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    //  Size = invoiceFactoring.path_file.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.payment_date
                };

                SqlParameter payable_amount = new SqlParameter
                {
                    ParameterName = "@payable_amount",
                    SqlDbType = System.Data.SqlDbType.Decimal,
                    //  Size = invoiceFactoring.path_file.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceFactoring.payable_amount
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
            id_enterprise,
            invoice_id,
            document_id,
            invoice_uuid,
            invoice_uuid_type,
            invoice_issuedate,
            supplier_type_identification,
            supplier_identification,
            customer_type_identification,
            customer_identification,
            path_file_xml,
            payment_date,
            payable_amount,
            id
                };

                //Insercion
                //Llamamos al SP y le pasamos los parametros
                int result = this
               .Database
               .ExecuteSqlRaw("EXEC uspInvoiceFactoringRecord " +
               "@id_enterprise, " +
               "@invoice_id, " +
               "@document_id, " +
               "@invoice_uuid, " +
               "@invoice_uuid_type, " +
               "@invoice_issuedate, " +
               "@supplier_type_identification, " +
               "@supplier_identification, " +
               "@customer_type_identification, " +
               "@customer_identification, " +
               "@path_file_xml, " +
               "@payment_date, " +
               "@payable_amount, " +
               "@id out",
               parameters);

                if (result == 1)
                { return result; }
                else
                {
                    string strObjt = id.Value.ToString();

                    if (strObjt != "")
                    {
                        int returnIdValue = Convert.ToInt32(strObjt);

                        if (returnIdValue == -1)
                        {
                            throw new Exception("Ya existe una Factura registrada con el identificador " + document_id.Value.ToString());
                        }
                    }

                    return result;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceStatusLast>> GetInvoiceStatusLastAsync(string documentId, string identification, string uuid, ILogMongo log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                SqlParameter document_idParam = new SqlParameter
                {
                    ParameterName = "@document_id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = documentId
                };

                SqlParameter invoiceUuidParam = new SqlParameter
                {
                    ParameterName = "@supplier_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = identification
                };

                SqlParameter supplierIdentificationParam = new SqlParameter
                {
                    ParameterName = "@invoice_uuid",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = uuid
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                    document_idParam,
                    supplierIdentificationParam,
                    invoiceUuidParam
                };

                string query = "EXEC dbo.uspInvoiceReadLastStatus @document_id, @supplier_identification, @invoice_uuid;";

                IEnumerable<InvoiceStatusLast> result = this.InvoiceStatusLast.FromSqlRaw(query, parameters).AsEnumerable();

                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, query, LevelMsn.Info, time.ElapsedMilliseconds);

                if (result != null)
                {
                    return result.ToList();
                }
                else
                {
                    return new List<InvoiceStatusLast>();
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                return new List<InvoiceStatusLast>();
            }
        }

        public async Task<bool> InvoiceStatusEventHistUpdateAsync(int id, DateTime updateAt, ILogMongo log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = id
                };

                SqlParameter updateAtParam = new SqlParameter
                {
                    ParameterName = "@updated_at",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = updateAt
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                    idParam,
                    updateAtParam
                };

                string query = "EXEC dbo.uspInvoiceStatusEventHistUpdate @id, @updated_at;";

                int result = this.Database.ExecuteSqlRaw(query, parameters);

                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, query, LevelMsn.Info, time.ElapsedMilliseconds);

                if (result == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }


        }

        public async Task<bool> InvoiceLastStatusCreateAsync(int id, InvoiceStatusLast statusEntity, ILogMongo log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {

                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output,
                    Value = id
                };

                SqlParameter statusParam = new SqlParameter
                {
                    ParameterName = "@status",
                    SqlDbType = System.Data.SqlDbType.SmallInt,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceStatus
                };

                SqlParameter documentIdParam = new SqlParameter
                {
                    ParameterName = "@document_id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceDocumentId
                };

                SqlParameter invoiceParam = new SqlParameter
                {
                    ParameterName = "@invoice_uuid",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceUuid
                };

                SqlParameter supplierIdentificationParam = new SqlParameter
                {
                    ParameterName = "@supplier_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceSupplierIdentification
                };

                SqlParameter supplierTypeIdentificationParam = new SqlParameter
                {
                    ParameterName = "@supplier_type_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceSupplierTypeIdentification
                };

                SqlParameter customerTypeIdentificationParam = new SqlParameter
                {
                    ParameterName = "@customer_type_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceCustomerTypeIdentification
                };

                SqlParameter customerIdentificationParam = new SqlParameter
                {
                    ParameterName = "@customer_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceCustomerIdentification
                };

                SqlParameter createdAtParam = new SqlParameter
                {
                    ParameterName = "@created_at",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceCreatedAt
                };

                SqlParameter updatedAtParam = new SqlParameter
                {
                    ParameterName = "@updated_at",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.InvoiceUpdatedAt
                };

                SqlParameter eStatusParam = new SqlParameter
                {
                    ParameterName = "@e_status",
                    SqlDbType = System.Data.SqlDbType.SmallInt,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.Status
                };

                SqlParameter dianStatusParam = new SqlParameter
                {
                    ParameterName = "@dian_status",
                    SqlDbType = System.Data.SqlDbType.SmallInt,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.DianStatus
                };

                SqlParameter dianMessageParam = new SqlParameter
                {
                    ParameterName = "@dian_message",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.DianMessage
                };

                SqlParameter eventIdParam = new SqlParameter
                {
                    ParameterName = "@event_id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.EventId
                };

                SqlParameter eventUUIDParam = new SqlParameter
                {
                    ParameterName = "@event_uuid",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.EventUuid
                };

                SqlParameter eventTypeParam = new SqlParameter
                {
                    ParameterName = "@event_type",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.EventType
                };

                SqlParameter eventDatetimeParam = new SqlParameter
                {
                    ParameterName = "@event_datetime",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.EventDatetime
                };

                SqlParameter eSupplierTypeIdentificationParam = new SqlParameter
                {
                    ParameterName = "@e_supplier_type_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.SupplierTypeIdentification
                };

                SqlParameter eSupplierIdentificationParam = new SqlParameter
                {
                    ParameterName = "@e_supplier_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.SupplierIdentification
                };

                SqlParameter eCustomerTypeIdentificationParam = new SqlParameter
                {
                    ParameterName = "@e_customer_type_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.CustomerTypeIdentification
                };

                SqlParameter eCustomerIdentificationParam = new SqlParameter
                {
                    ParameterName = "@e_customer_identification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.CustomerIdentification
                };

                SqlParameter eCreatedAtParam = new SqlParameter
                {
                    ParameterName = "@e_created_at",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.CreatedAt
                };

                SqlParameter eUpdatedAtParam = new SqlParameter
                {
                    ParameterName = "@e_updated_at",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.UpdatedAt
                };

                SqlParameter supplierRegistrationNameParam = new SqlParameter
                {
                    ParameterName = "@supplier_registration_name",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.SupplierRegistrationName
                };

                SqlParameter customerRegistrationNameParam = new SqlParameter
                {
                    ParameterName = "@customer_registration_name",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.CustomerRegistrationName
                };

                SqlParameter referenceIdParam = new SqlParameter
                {
                    ParameterName = "@reference_id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.ReferenceId
                };

                SqlParameter eventUUIDTypeParam = new SqlParameter
                {
                    ParameterName = "@event_uuid_type",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = statusEntity.EventUuidType
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                    idParam,
                    statusParam,
                    documentIdParam,
                    invoiceParam,
                    supplierIdentificationParam,
                    supplierTypeIdentificationParam,
                    customerTypeIdentificationParam,
                    customerIdentificationParam,
                    createdAtParam,
                    updatedAtParam,
                    eStatusParam,
                    dianStatusParam,
                    dianMessageParam,
                    eventIdParam,
                    eventUUIDParam,
                    eventTypeParam,
                    eventDatetimeParam,
                    eSupplierTypeIdentificationParam,
                    eSupplierIdentificationParam,
                    eCustomerTypeIdentificationParam,
                    eCustomerIdentificationParam,
                    eCreatedAtParam,
                    eUpdatedAtParam,
                    supplierRegistrationNameParam,
                    customerRegistrationNameParam,
                    referenceIdParam,
                    eventUUIDTypeParam
                };

                string query = "EXEC dbo.uspInvoiceLastStatusCreate @id, @status, @document_id, @invoice_uuid, @supplier_identification," +
                    " @supplier_type_identification, @customer_type_identification, @customer_identification," +
                    " @created_at, @updated_at, @e_status, @dian_status, @dian_message, @event_id, @event_uuid," +
                    " @event_type, @event_datetime, @e_supplier_type_identification, @e_supplier_identification," +
                    " @e_customer_type_identification, @e_customer_identification, @e_created_at, @e_updated_at," +
                    " @supplier_registration_name, @customer_registration_name, @reference_id, @event_uuid_type";

                int result = this.Database.ExecuteSqlRaw(query, parameters);

                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, query, LevelMsn.Info, time.ElapsedMilliseconds);

                if (result >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }
        }

        #endregion
    }
}