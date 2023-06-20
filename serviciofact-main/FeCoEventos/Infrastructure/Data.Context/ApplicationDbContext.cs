using FeCoEventos.Domain.Entity;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Util.TableLog;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TFHKA.EventsDian.Infrastructure.Data.Context
{
    /// <summary>
    /// Clase Context para manejo de DB
    /// </summary>
    public partial class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IConfiguration _configuration;

        #region Constructores

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /* public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
         {

         }*/
        #endregion

        #region Propiedades
        public DbSet<InvoiceEventTable> InvoiceEvents { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventSummary> EventSummary { get; set; }

        public DbSet<EventCount> EventCount { get; set; }

        #endregion

        #region Sobrecargas
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:DefaultConnection"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Para uso de Migrations
            modelBuilder.Entity<InvoiceEventTable>(entity =>
            {
                entity.Property(e => e.id).HasColumnName("id");

                entity.Property(e => e.document_id)
              .IsRequired()
              .HasMaxLength(20);

                entity.Property(e => e.invoice_uuid)
              .IsRequired()
              .HasMaxLength(200);

                entity.Property(e => e.invoice_uuid_type)
              .IsRequired()
              .HasMaxLength(20);

                entity.Property(e => e.invoice_issuedate)
              .IsRequired();

                entity.ToTable("invoice_events");
            });

            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #endregion



        #region Implementaciones
        /// <summary>
        /// Metodo de implementacion de SP de Persistencia y Actualizaciones
        /// </summary>
        /// <param name="invoiceEvent">Entidad con los valores</param>
        /// <returns></returns>
        public int SaveUpdateEventInvoice(InvoiceEventTable invoiceEvent, byte operationType, IConfiguration configuration, ILogAzure log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                if (operationType == 0)
                {
                    // Settings Parameters Input.         
                    SqlParameter id_enterprise = new SqlParameter
                    {
                        ParameterName = "@id_enterprise",
                        SqlDbType = System.Data.SqlDbType.Int,
                        // Size = invoiceEvent.id_enterprise.ToString().Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.id_enterprise
                    };

                    SqlParameter invoice_id = new SqlParameter
                    {
                        ParameterName = "@invoice_id",
                        SqlDbType = System.Data.SqlDbType.Int,
                        // Size = invoiceEvent.invoice_id.ToString().Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.invoice_id
                    };

                    SqlParameter document_id = new SqlParameter
                    {
                        ParameterName = "@document_id",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        // Size = invoiceEvent.document_id.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.document_id
                    };

                    SqlParameter invoice_uuid = new SqlParameter
                    {
                        ParameterName = "@invoice_uuid",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.invoice_uuid.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.invoice_uuid
                    };

                    SqlParameter invoice_uuid_type = new SqlParameter
                    {
                        ParameterName = "@invoice_uuid_type",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.invoice_uuid_type.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.invoice_uuid_type
                    };

                    SqlParameter invoice_issuedate = new SqlParameter
                    {
                        ParameterName = "@invoice_issuedate",
                        SqlDbType = System.Data.SqlDbType.DateTime,
                        //  Size = invoiceEvent.invoice_issuedate.ToString().Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.invoice_issuedate
                    };

                    SqlParameter supplier_type_identification = new SqlParameter
                    {
                        ParameterName = "@supplier_type_identification",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.supplier_type_identification.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.supplier_type_identification
                    };

                    SqlParameter supplier_identification = new SqlParameter
                    {
                        ParameterName = "@supplier_identification",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.supplier_identification.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.supplier_identification
                    };

                    SqlParameter customer_type_identification = new SqlParameter
                    {
                        ParameterName = "@customer_type_identification",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.customer_type_identification.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.customer_type_identification
                    };

                    SqlParameter customer_identification = new SqlParameter
                    {
                        ParameterName = "@customer_identification",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.customer_identification.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.customer_identification
                    };

                    SqlParameter event_id = new SqlParameter
                    {
                        ParameterName = "@event_id",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        // Size = invoiceEvent.event_id.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.event_id
                    };

                    SqlParameter event_type = new SqlParameter
                    {
                        ParameterName = "@event_type",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        // Size = invoiceEvent.event_type.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.event_type
                    };

                    SqlParameter event_uuid = new SqlParameter
                    {
                        ParameterName = "@event_uuid",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.event_uuid.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.event_uuid
                    };

                    SqlParameter event_uuid_type = new SqlParameter
                    {
                        ParameterName = "@event_uuid_type",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.event_uuid_type.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.event_uuid_type
                    };

                    SqlParameter path_file = new SqlParameter
                    {
                        ParameterName = "@path_file",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.path_file.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.path_file
                    };

                    SqlParameter namefile = new SqlParameter
                    {
                        ParameterName = "@namefile",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.namefile.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.namefile
                    };

                    SqlParameter session_log = new SqlParameter
                    {
                        ParameterName = "@session_log",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        // Size = invoiceEvent.session_log.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.session_log
                    };

                    SqlParameter track_id = new SqlParameter
                    {
                        ParameterName = "@track_id",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.track_id.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.track_id
                    };

                    SqlParameter email = new SqlParameter
                    {
                        ParameterName = "@email",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        //  Size = invoiceEvent.track_id.Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.email != null ? invoiceEvent.email : ""
                    };

                    SqlParameter environment = new SqlParameter
                    {
                        ParameterName = "@environment",
                        SqlDbType = System.Data.SqlDbType.Int,
                        // Size = invoiceEvent.invoice_id.ToString().Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.environment
                    };

                    SqlParameter created_by = new SqlParameter
                    {
                        ParameterName = "@created_by",
                        SqlDbType = System.Data.SqlDbType.SmallInt,
                        // Size = invoiceEvent.invoice_id.ToString().Length,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.created_by
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
                      event_id,
                      event_type,
                      event_uuid,
                      event_uuid_type,
                      path_file,
                      namefile,
                      session_log,
                      track_id,
                      email,
                      environment,
                      created_by,
                      id
                    };

                    //Insercion
                    //Llamamos al SP y le pasamos los parametros
                    using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                    {
                        int result = dbo.Database
                    .ExecuteSqlRaw("exec uspEventRecord " +
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
                    "@event_type, " +
                    "@event_id, " +
                    "@event_uuid, " +
                    "@event_uuid_type, " +
                    "@path_file, " +
                    "@namefile, " +
                    "@session_log, " +
                    "@track_id, " +
                    "@email, " +
                    "@environment, " +
                    "@created_by," +
                    "@id out",
                    parameters);

                        time.Stop();
                        log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);

                        return result;
                    }
                }
                else
                {
                    SqlParameter id = new SqlParameter
                    {
                        ParameterName = "@id",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Direction = System.Data.ParameterDirection.Output
                    };

                    SqlParameter status = new SqlParameter
                    {
                        ParameterName = "@status",
                        SqlDbType = System.Data.SqlDbType.SmallInt,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.status
                    };

                    SqlParameter active = new SqlParameter
                    {
                        ParameterName = "@active",
                        SqlDbType = System.Data.SqlDbType.Bit,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.active
                    };

                    SqlParameter event_id = new SqlParameter
                    {
                        ParameterName = "@event_id",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.event_id
                    };

                    SqlParameter dian_status = new SqlParameter
                    {
                        ParameterName = "@dian_status",
                        SqlDbType = System.Data.SqlDbType.SmallInt,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.dian_status
                    };

                    SqlParameter dian_message = new SqlParameter
                    {
                        ParameterName = "@dian_message",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.dian_message
                    };

                    SqlParameter dian_response_datetime = new SqlParameter
                    {
                        ParameterName = "@dian_response_datetime",
                        SqlDbType = System.Data.SqlDbType.DateTime,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.dian_response_datetime
                    };

                    SqlParameter track_id = new SqlParameter
                    {
                        ParameterName = "@track_id",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.track_id
                    };

                    SqlParameter dian_result_pathfile = new SqlParameter
                    {
                        ParameterName = "@dian_result_pathfile",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.dian_result_pathfile
                    };

                    SqlParameter dian_result_namefile = new SqlParameter
                    {
                        ParameterName = "@dian_result_namefile",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.dian_result_namefile
                    };

                    SqlParameter dian_result_validation = new SqlParameter
                    {
                        ParameterName = "@dian_result_validation",
                        SqlDbType = System.Data.SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.dian_result_validation
                    };
                    SqlParameter tries_send = new SqlParameter
                    {
                        ParameterName = "@tries_send",
                        SqlDbType = System.Data.SqlDbType.Int,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.tries_send
                    };

                    SqlParameter try_query = new SqlParameter
                    {
                        ParameterName = "@try_query",
                        SqlDbType = System.Data.SqlDbType.TinyInt,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = invoiceEvent.try_query
                    };

                    SqlParameter[] parameters = new SqlParameter[]
                    {id, status, active, event_id, dian_status, dian_message, dian_response_datetime, track_id, dian_result_pathfile,
                        dian_result_namefile, dian_result_validation, tries_send, try_query};

                    using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                    {
                        int result = dbo.Database.ExecuteSqlRaw("exec uspUpdateResponseDIAN " + "@id out, " + "@status, " + "@active, " +
                        "@event_id, " + "@dian_status, " + "@dian_message, " + "@dian_response_datetime, " + "@track_id, " +
                        "@dian_result_pathfile, " + "@dian_result_namefile, " + "@dian_result_validation, "
                        + "@tries_send, " + "@try_query", parameters);

                        time.Stop();
                        log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }
        }




        /// <summary>
        /// Consultar en invoice_events si existe un registro con 
        /// </summary>
        /// <param name="status">Estatus de los Eventos</param>
        /// <param name="evenCode">Codigo de los Eventso</param>
        /// <param name="dateFrom">Fecha Inicial</param>
        /// <param name="dateEnd">Fecha Final</param>
        /// <returns></returns>
        public List<InvoiceEventTable> GetAnysInvoiceEvents(int status, string evenCode, DateTime dateFrom, DateTime dateEnd, ILogAzure log, IConfiguration configuration)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                // Settings Parameters Input.         
                SqlParameter evenCodeParam = new SqlParameter
                {
                    ParameterName = "@EventType",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = evenCode.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = evenCode
                };

                SqlParameter statusParam = new SqlParameter
                {
                    ParameterName = "@Status",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = status
                };

                SqlParameter dateFromParam = new SqlParameter
                {
                    ParameterName = "@InitialDate",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Size = dateFrom.ToString().Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = dateFrom
                };

                SqlParameter dateEndParam = new SqlParameter
                {
                    ParameterName = "@FinalDate",
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    Size = dateEnd.ToString().Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = dateEnd
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                  //  limitParam,
                    dateFromParam,
                    dateEndParam,
                    evenCodeParam,
                    statusParam
                };

                //Llamamos al SP y le pasamos los parametros
                using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                {
                    List<InvoiceEventTable>? resultList = dbo.InvoiceEvents
                           .FromSqlRaw("EXEC uspGetEvents " +
                           "@InitialDate, " +
                           "@FinalDate, " +
                           "@EventType, " +
                           "@Status; ",
                                    parameters)
                           .ToListAsync().Result;

                    time.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);

                    return resultList;
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, "SQL.Exception:" + JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                return null;
            }
        }


        /// <summary>
        /// Metodo de implementacion para consumumir el SP Consultar en invoice_events si existe un registro con event_uuid y event_id
        /// </summary>
        /// <param name="eventUuid">HASH identificador del evento</param>
        /// <param name="eventId">Identificador del evento en la tabla invoice_events</param>
        /// <returns></returns>
        public async Task<EventCount> GetInvoiceEventsReadByEventUuid(string eventId, string eventUuid, IConfiguration configuration, ILogAzure log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                //setting parameters input 
                SqlParameter eventIdParam = new SqlParameter
                {
                    ParameterName = "@event_id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = eventId.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = eventId
                };
                SqlParameter eventUuidParam = new SqlParameter
                {
                    ParameterName = "@event_uuid",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = eventUuid.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = eventUuid
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                        eventIdParam,
                        eventUuidParam
                };

                using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                {
                    IEnumerable<EventCount>? invoiceEventsReadByEventUuid = dbo.EventCount.FromSqlRaw("EXEC dbo.usp_InvoiceEventsReadByEventUuid @event_uuid, @event_id;",
                            parameters)
                            .AsEnumerable();

                    time.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);
                    return invoiceEventsReadByEventUuid.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }
        }


        /// <summary>
        /// Metodo de implementacion para consumumir el SP de consulta de un Evento especifico por su EventId y TrackId
        /// </summary>
        /// <param name="eventId">EventId</param>
        /// <param name="trackId">TrackId</param>
        /// <returns></returns>
        public async Task<Event> GetEventStatus(string eventId, string trackId, IConfiguration configuration, ILogAzure log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                // Settings Parameters Input.         
                SqlParameter eventIdParam = new SqlParameter
                {
                    ParameterName = "@EventId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = eventId.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = eventId
                };

                SqlParameter trackIdParam = new SqlParameter
                {
                    ParameterName = "@TrackId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = trackId.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = trackId
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                        eventIdParam,
                        trackIdParam
                };

                using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                {
                    IEnumerable<Event>? eventStatus = dbo.Events.FromSqlRaw("EXEC dbo.uspGetEventStatus @EventId, @TrackId;",
                            parameters)
                            .AsEnumerable();

                    time.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);

                    return eventStatus.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }
        }

        public int UpdateEventAsyncSend(InvoiceEventTable invoiceEvent, string dianTrackId, IConfiguration configuration, ILogAzure log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                SqlParameter eventId = new SqlParameter
                {
                    ParameterName = "@eventId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.event_id
                };

                SqlParameter trackId = new SqlParameter
                {
                    ParameterName = "@trackId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.track_id
                };

                SqlParameter status = new SqlParameter
                {
                    ParameterName = "@status",
                    SqlDbType = System.Data.SqlDbType.SmallInt,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.status
                };

                SqlParameter trackIdDian = new SqlParameter
                {
                    ParameterName = "@trackIdDian",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = dianTrackId
                };

                //Parameter Output
                SqlParameter id = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                SqlParameter[] parameters = new SqlParameter[]
                {
                eventId,
                trackId,
                status,
                trackIdDian,
                id
                };

                using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                {
                    int result = dbo.Database.ExecuteSqlRaw("EXEC uspEventUpdateSendAsync @eventId, @trackId, @trackIdDian, @status, @id out;",
                    parameters);

                    time.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);

                    return result;
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }
        }


        public List<EventSummary> GetAllEventsEnable(string numberIdentification, IConfiguration configuration, ILogAzure log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                // Settings Parameters Input.         
                SqlParameter numberIdentificationParam = new SqlParameter
                {
                    ParameterName = "@NumberIdentification",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = numberIdentification.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = numberIdentification
                };

                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                        numberIdentificationParam
                };

                //Llamamos al SP y le pasamos los parametros
                using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                {
                    List<EventSummary>? eventStatus = dbo.EventSummary
                            .FromSqlRaw("EXEC dbo.uspGetEventEnableDian @NumberIdentification;",
                            parameters)
                            .ToList();

                    time.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);

                    return eventStatus;
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }
        }

        public int UpdateAttachedDocument(InvoiceEventTable invoiceEvent, IConfiguration configuration, ILogAzure log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                // Settings Parameters Input.
                SqlParameter id = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                SqlParameter eventIdParam = new SqlParameter
                {
                    ParameterName = "@eventId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = invoiceEvent.event_id.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.event_id
                };

                SqlParameter trackIdParam = new SqlParameter
                {
                    ParameterName = "@trackId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = invoiceEvent.track_id.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.track_id
                };

                SqlParameter attachedDocumentNamefileParam = new SqlParameter
                {
                    ParameterName = "@attacheddocument_namefile",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = invoiceEvent.attacheddocument_namefile.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.attacheddocument_namefile
                };

                SqlParameter attachedDocumentPathfileParam = new SqlParameter
                {
                    ParameterName = "@attacheddocument_pathfile",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = invoiceEvent.attacheddocument_pathfile.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.attacheddocument_pathfile
                };


                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                        id, eventIdParam, trackIdParam, attachedDocumentNamefileParam, attachedDocumentPathfileParam
                };

                //Llamamos al SP y le pasamos los parametros
                using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                {
                    int result = dbo.Database.ExecuteSqlRaw("EXEC dbo.uspUpdateEventAD @id out, @eventId, @trackId, @attacheddocument_namefile, @attacheddocument_pathfile;",
                            parameters);

                    time.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);

                    return result;
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }
        }

        public int UpdateFileResponseDian(InvoiceEventTable invoiceEvent, IConfiguration configuration, ILogAzure log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                // Settings Parameters Input.
                SqlParameter id = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                SqlParameter eventIdParam = new SqlParameter
                {
                    ParameterName = "@eventId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = invoiceEvent.event_id.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.event_id
                };

                SqlParameter trackIdParam = new SqlParameter
                {
                    ParameterName = "@trackId",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = invoiceEvent.track_id.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.track_id
                };

                SqlParameter DianNamefileParam = new SqlParameter
                {
                    ParameterName = "@dian_result_namefile",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = invoiceEvent.dian_result_namefile.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.dian_result_namefile
                };

                SqlParameter DianPathfileParam = new SqlParameter
                {
                    ParameterName = "@dian_result_pathfile",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Size = invoiceEvent.dian_result_pathfile.Length,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = invoiceEvent.dian_result_pathfile
                };


                //Arreglo de todos los parametros
                SqlParameter[] parameters = new SqlParameter[]
                {
                        id, eventIdParam, trackIdParam, DianNamefileParam, DianPathfileParam
                };

                using (ApplicationDbContext? dbo = new ApplicationDbContext(configuration))
                {
                    //Llamamos al SP y le pasamos los parametros
                    int result = dbo.Database.ExecuteSqlRaw("EXEC dbo.uspUpdateEventARDIAN @id out, @eventId, @trackId, @dian_result_namefile, @dian_result_pathfile;",
                            parameters);

                    time.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Ejecutada", LevelMsn.Info, time.ElapsedMilliseconds);

                    return result;
                }
            }
            catch (Exception ex)
            {
                time.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, time.ElapsedMilliseconds);

                throw ex;
            }
        }

        #endregion
    }
}