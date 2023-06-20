using FeCoEventos.Domain.Interface;
using FeCoEventos.Infrastructure.Data.Context;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util.TableLog;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FeCoEventos.Domain.Core
{
    public class EnableSummaryDomain : IEnableSummaryDomain
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public EnableSummaryDomain(IApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public EventsSummaryResponse GetList(string nit, ILogAzure log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                //Consulto los documentos
                var result = _context.GetAllEventsEnable(nit, _configuration, log);

                if (result == null)
                {
                    return new EventsSummaryResponse
                    {
                        Code = 500,
                        Message = "No se obtuvo un resultado de la consulta"
                    };
                }

                EventsSummaryResponse summary = new EventsSummaryResponse
                {
                    Code = 200,
                    Message = "Se retorna resumen de habilitacion del contribuyente",
                    Events = new List<EventSummary>()
                };

                List<string> caseEventEnable = new List<string> { "030", "031", "032", "033", "034", "035", "036", "037", "038", "039", "040", "041", "042", "043", "044", "045" };

                foreach (var row in caseEventEnable)
                {
                    //Extraer 031
                    string codeEvent = row;

                    var listCase = result.Where(x => x.EventType == codeEvent).ToList();

                    var summaryEvent = new EventSummary
                    {
                        CodeEvent = codeEvent,
                        Summary = new Summary(),
                        List = new List<EventItem>()
                    };

                    if (listCase.Count > 0)
                    {
                        //Contadores
                        summaryEvent.Summary.Success = listCase.Where(x => x.Status == 200).Count();
                        summaryEvent.Summary.Rejected = listCase.Where(x => x.Status == 99).Count();
                        summaryEvent.Summary.Pending = listCase.Where(x => x.Status == 201 || x.Status == 204).Count();

                        var ListNotError = new List<int> { 200, 201, 204, 99 };

                        summaryEvent.Summary.Error = listCase.Where(x => !ListNotError.Contains(x.Status)).Count();

                        //Listado de Casos
                        foreach (var e in listCase)
                        {
                            EventItem item = new EventItem
                            {
                                InvoiceId = e.DocumentId,
                                InvoiceCufe = e.InvoiceUuid,
                                EventId = e.EventId,
                                EventCufe = e.EventUuid,
                                TrackId = e.TrackId,
                                StatusDian = e.DianStatus.HasValue ? (int)e.DianStatus.Value : 0,
                                Status = (int)e.Status,
                                Type = e.EventType
                            };

                            summaryEvent.List.Add(item);
                        }
                    }

                    summary.Events.Add(summaryEvent);

                    log.WriteComment(MethodBase.GetCurrentMethod().Name, codeEvent + "Ok", LevelMsn.Info, timeT.ElapsedMilliseconds);
                }

                return summary;
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name + ".Exception", JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                return new EventsSummaryResponse
                {
                    Code = 500,
                    Message = "No se obtuvo un resultado de la consulta"
                };
            }
        }
    }
}
