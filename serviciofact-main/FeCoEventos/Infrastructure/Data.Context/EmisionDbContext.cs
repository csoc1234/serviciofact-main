using FeCoEventos.Util.TableLog;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;

namespace FeCoEventos.Infrastructure.Data.Context
{
    public partial class EmisionDbContext : DbContext, IEmisionDbContext
    {
        private readonly IConfiguration _configuration;

        #region Atributos Globales

        private readonly string _connectionString;

        #endregion


        #region Constructores

        /*
        public EmisionDbContext(DbContextOptions<EmisionDbContext> options) : base(options)
        {

        }*/

        public EmisionDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:EmisionConnection"]);
            }
        }

        #endregion

        #region Implementaciones
        public int UpdateInvoiceHistoryEvent(int dianStatus, string eventId, string trackId, bool active, IConfiguration configuration, ILogAzure log)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                SqlParameter dianStatusParam = new SqlParameter
                {
                    ParameterName = "@status_dian",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = dianStatus
                };

                SqlParameter eventIdParam = new SqlParameter
                {
                    ParameterName = "@event_id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = eventId
                };

                SqlParameter trackIdParam = new SqlParameter
                {
                    ParameterName = "@tracking_id",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = trackId
                };

                SqlParameter activeParam = new SqlParameter
                {
                    ParameterName = "@active",
                    SqlDbType = System.Data.SqlDbType.Bit,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = active
                };

                SqlParameter codigoParam = new SqlParameter
                {
                    ParameterName = "@codigo",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                SqlParameter[] parameters = new SqlParameter[]
                {
                dianStatusParam, eventIdParam, trackIdParam, activeParam, codigoParam
                };

                using (EmisionDbContext? dbo = new EmisionDbContext(configuration))
                {
                    int result = dbo.Database.ExecuteSqlRaw("exec sp_update_invoice_reception_history_event " +
                   "@status_dian, " +
                   "@event_id, " +
                   "@tracking_id, " +
                   "@active, " +
                   "@codigo out",
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

        public int UpdateReceptionStatus(string uuid, int estatus, IConfiguration configuration)
        {
            Stopwatch time = new Stopwatch();
            time.Start();

            try
            {
                SqlParameter uuidParam = new SqlParameter
                {
                    ParameterName = "@uuid",
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = uuid
                };

                SqlParameter estatusParam = new SqlParameter
                {
                    ParameterName = "@estatus",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = estatus
                };

                SqlParameter[] parameters = new SqlParameter[]
               {
               uuidParam, estatusParam
               };

                using (EmisionDbContext? dbo = new EmisionDbContext(configuration))
                {
                    int result = dbo.Database.ExecuteSqlRaw("UPDATE dbo.invoice_reception SET [last_constraint_status] = @estatus, status = @estatus WHERE uuid = @uuid;",
                   parameters);

                    time.Stop();

                    return result;
                }
            }
            catch (Exception ex)
            {
                time.Stop();

                throw ex;
            }
        }

        #endregion

    }
}
