using FeCoEventos.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using TFHKA.AzureStorageLibrary.Loggin;
using static FeCoEventos.Util.TableLog.LogAzure;

namespace FeCoEventos.Util.TableLog
{
    public class LogAzure : ILogAzure
    {
        public enum ApplicationType
        {
            None = 0,
            Portal = 1,
            Integracion = 2,
            AppMovil = 3
        }
        private string AccountName;
        private string AccountKey;
        private string AppId;
        private string Level;
        private string zoneTime;
        private Logger<ObjEntry> Log;
        public ObjEntry EntryLog;
        private List<StepProcess> pasos;
        private static IConfiguration _configuration;

        public LogAzure(IConfiguration configuration)
        {
            _configuration = configuration;
            pasos = new List<StepProcess>();
            EntryLog = new ObjEntry();
            LoadConfig();
            Log = new Logger<ObjEntry>(AppId, AccountName, AccountKey, "1", level: Level);

        }

        public void SetUp(CustomJwtTokenContext context, LogRequest logRequest)
        {
            pasos = new List<StepProcess>();
            LoadConfig();
            EntryLog = new ObjEntry(zoneTime);
            EntryLog.RowKeyTime = ColombiaTimezone();
            EntryLog.PartitionKey = context.User.EnterpriseToken;
            EntryLog.NITSolicitante = context.User.EnterpriseNit;
            EntryLog.NameMethod = logRequest.Method;
            EntryLog.Session = Guid.NewGuid().ToString();
            EntryLog.Application = logRequest.Application.ToString();
            EntryLog.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            EntryLog.Api = logRequest.Api;
            Log = new Logger<ObjEntry>(AppId, AccountName, AccountKey, EntryLog.PartitionKey, level: Level);
        }

        private void LoadConfig()
        {
            AccountName = _configuration["StorageAzure:AccountName"];
            AccountKey = _configuration["StorageAzure:AccountKey"];
            AppId = _configuration["StorageAzure:Tablename"] + DateTime.Now.ToString("yyyyMM");
            zoneTime = _configuration["TimeZones:TimeZoneColombia"];

            string readLevel = _configuration["StorageAzure:LogLevel"];
            switch (readLevel)
            {
                case "Error":
                    Level = LogLevel.Error;
                    break;
                case "Notice":
                    Level = LogLevel.Notice;
                    break;
                case "Off":
                    Level = LogLevel.Off;
                    break;
                case "Warning":
                    Level = LogLevel.Error;
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pname"></param>
        /// <param name="pcomment"></param>
        /// <param name="pLevel"></param>
        /// <param name="timeElapse"></param>
        public void WriteComment(string pname, string pcomment, LevelMsn pLevel = LevelMsn.Info, double timeElapse = 0)
        {
            try
            {
                pasos.Add(new StepProcess { HoraProcess = LogAzure.ColombiaTimezone(), NameProcess = pname, Comment = pcomment, LevelInfo = pLevel.ToString(), TimeElapse = timeElapse });
            }
            catch (Exception)
            {

            }
        }

        public void SaveLog(int codigo, string comment, ref Stopwatch time, LevelMsn level = LevelMsn.Info, byte[] document = null)
        {
            try
            {
                time.Stop();

                lock (EntryLog)
                {
                    EntryLog.Process = ConvertToJson(pasos);
                    EntryLog.Codigo = codigo;
                    EntryLog.Comment = comment;
                    EntryLog.Level = level.ToString();
                    EntryLog.IpAddress = GetLocalIPAddress();
                    EntryLog.ElapsedTime = time.ElapsedMilliseconds;

                    Log.Add(EntryLog, EntryLog.Level);
                    pasos.Clear();
                    EntryLog.ClearEntry();
                }
            }
            catch (Exception en)
            {
                pasos.Clear();
                EntryLog.ClearEntry();
            }
        }

        public string GetSession()
        {
            return EntryLog.Session;
        }
        public static string ConvertToJson(object objtoConvert)
        {
            try
            {
                return JsonConvert.SerializeObject(objtoConvert);
            }
            catch (Exception en)
            {
                return "Error en ConvertToJson";
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static DateTime ColombiaTimezone()
        {
            string time = _configuration["TimeZones:TimeZoneColombia"];
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(time);

            DateTime dateColombia = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone);
            return dateColombia;
        }
    }

    public class ObjEntry : LogEntry
    {
        public ObjEntry() : base()
        {

        }

        public ObjEntry(string zoneTime) : base(zoneTime)
        {

        }

        public string NameMethod { get; set; }
        public string NITSolicitante { get; set; }
        public string NitEmisor { get; set; }
        public string NumeroDocumento { get; set; }

        public double ElapsedTime { get; set; }
        public string Session { get; set; }
        public int Codigo { get; set; }
        public string Comment { get; set; }
        public string DocumentId { get; set; }
        public string Version { get; set; }
        public string PathFile { get; set; }
        public string IpAddress { get; set; }
        public string Application { get; set; }
        public string Request { get; set; }
        public string XmlLog { get; set; }

        public string Api { get; set; }

        public void ClearEntry()
        {
            this.NameMethod = "";
            this.NITSolicitante = "";
            this.NitEmisor = "";
            this.NumeroDocumento = "";
            this.ElapsedTime = 0;
            this.Session = "";
            this.Codigo = 0;
            this.Comment = "";
            this.DocumentId = "";
            this.Version = "";
            this.PathFile = "";
            this.Process = "";
            this.IpAddress = "";
            this.Application = "";
            this.Request = "";
            this.XmlLog = "";
        }
    }

    public enum LevelMsn
    {
        Info = 1,
        Warning,
        Error
    }

    public class StepProcess
    {
        public double TimeElapse { get; set; }
        public DateTime HoraProcess { get; set; }
        public string NameProcess { get; set; }
        public string Comment { get; set; }
        public string LevelInfo { get; set; }

        Enum levelMsn;
    }

    public class LogRequest
    {
        public CustomJwtTokenContext Context { get; set; }
        public string Method { get; set; }
        public string Api { get; set; }
        public ApplicationType Application { get; set; }
    }
}
