using FeCoEventos.Domain.Interface;
using FeCoEventos.Infrastructure.AzureStorage;
using FeCoEventos.Infrastructure.AzureStorage.Interface;
using FeCoEventos.Util.TableLog;
using Newtonsoft.Json;
using System;
using System.Reflection;
using TFHKA.EventsDian.Infrastructure.Data.Context;

namespace FeCoEventos.Domain.Core
{
    public class EventFileDomain : IEventFileDomain
    {
        private readonly IStorageFiles _storageFiles;

        public EventFileDomain(IStorageFiles storageFiles)
        {
            _storageFiles = storageFiles;
        }

        /// <summary>
        /// Se busca en el storage el archivo XML del Evento
        /// </summary>
        /// <param name="eventTable">Datos del registro de un evento</param>
        /// <param name="log">Log</param>
        /// <returns>Archivo xml codificado en base64</returns>
        public string GetFileXml(InvoiceEventTable eventTable, ILogAzure log)
        {
            try
            {
                var fileResponse = _storageFiles.GetFile(eventTable.path_file, eventTable.namefile, StorageConfiguration.FactoringFileShare, log);

                if (fileResponse != null)
                {
                    if (fileResponse.Code == 200)
                    {
                        if (!string.IsNullOrEmpty(fileResponse.File))
                        {
                            return fileResponse.File;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else
                    {
                        log.WriteComment(MethodBase.GetCurrentMethod().Name, fileResponse.Message, LevelMsn.Error);
                        return string.Empty;
                    }
                }
                else
                {
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, $"Error al momento de conectarse al Storage. Ruta:{eventTable.path_file} Archivo:{eventTable.namefile}", LevelMsn.Error);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error);
                return string.Empty;
            }
        }

    }
}
