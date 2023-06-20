using FeCoEventos.Domain.Interface;
using FeCoEventos.Domain.ValueObjects;
using FeCoEventos.Infrastructure.SiteRemote.Interface;
using FeCoEventos.Models.Requests;
using FeCoEventos.Models.Responses;
using FeCoEventos.Util;
using FeCoEventos.Util.TableLog;
using System;
using System.Xml;
using TFHKA.Storage.Fileshare.Client.Models;

namespace FeCoEventos.Domain.Core
{
    public class FilesDomain : IFilesDomain
    {
        private readonly IValidationXMLClient _validationXMLClient;
        private readonly IAttachedDocumentClient _attachedDocumentClient;
        private readonly IEventXmlClient _eventXMLClient;
        private readonly IDocumentBuild _documentBuild;

        public FilesDomain(IValidationXMLClient validationXMLClient,
            IDocumentBuild documentBuild,
            IEventXmlClient eventXMLClient,
            IAttachedDocumentClient attachedDocumentClient)
        {
            _validationXMLClient = validationXMLClient;
            _eventXMLClient = eventXMLClient;
            _documentBuild = documentBuild;
            _attachedDocumentClient = attachedDocumentClient;
        }

        public StorageFileResponse GetXmlDian(string uuid, ILogAzure log)
        {
            //Intento buscarlo en la DIAN el XML de Aceptacion del Evento
            var resultXmlDianApi = _eventXMLClient.GetEventXML(uuid, log);

            if (resultXmlDianApi.Code == 200)
            {
                return new StorageFileResponse
                {
                    Code = resultXmlDianApi.Code,
                    Message = resultXmlDianApi.Message,
                    File = resultXmlDianApi.ApplicationResponse
                };
            }
            else if (resultXmlDianApi.Code == 100)
            {
                return new StorageFileResponse
                {
                    Code = resultXmlDianApi.Code,
                    Message = "No se ha encontrado evento en la DIAN"
                };
            }
            else
            {
                return new StorageFileResponse
                {
                    Code = resultXmlDianApi.Code,
                    Message = resultXmlDianApi.Message
                };
            }
        }

        public StorageFileResponse GetXmlValidationDian(string uuid, ILogAzure log)
        {
            //Intento buscarlo en la DIAN el XML de Aceptacion del Evento
            var resultXmlDianApi = _validationXMLClient.GetValidationXML(uuid, log);

            if (resultXmlDianApi.Code == 200)
            {
                return new StorageFileResponse
                {
                    Code = resultXmlDianApi.Code,
                    Message = resultXmlDianApi.Message,
                    File = resultXmlDianApi.ApplicationResponse
                };
            }
            else if (resultXmlDianApi.Code == 100)
            {
                return new StorageFileResponse
                {
                    Code = resultXmlDianApi.Code,
                    Message = "No se ha encontrado evento en la DIAN"
                };
            }
            else
            {
                return new StorageFileResponse
                {
                    Code = resultXmlDianApi.Code,
                    Message = resultXmlDianApi.Message
                };
            }
        }

        public FileXmlResponse ParseResponse(StorageFileResponse resultFile, string prefix, string supplierIdentification)
        {
            FileXmlResponse fileXmlResponse;

            if (resultFile.Code == 200)
            {
                //Get Name 
                string name = GetNameFile(prefix, supplierIdentification, resultFile.File);

                fileXmlResponse = new FileXmlResponse
                {
                    Code = 200,
                    Message = "Se retorna archivo solicitado",
                    FileXml = new Entity.FileXml
                    {
                        File = resultFile.File,
                        Name = name
                    }
                };
            }
            else
            {
                fileXmlResponse = new FileXmlResponse
                {
                    Code = resultFile.Code,
                    Message = resultFile.Message
                };
            }

            return fileXmlResponse;
        }

        public FileXmlResponse BuildAttachedDocument(AttachedDocumentRequest request, string tokenJwt, string supplierIdentification, ILogAzure log)
        {
            var result = _attachedDocumentClient.GenerateXml(request, tokenJwt, log);

            var resultFileAD = new FileXmlResponse
            {
                Code = result.Code,
                Message = result.Message
            };

            if (result.Code == 200)
            {
                resultFileAD.FileXml = new Entity.FileXml
                {
                    File = result.File,
                    Name = result.Namefile
                };
            }

            return resultFileAD;
        }

        public string GetNameFile(string prefix, string SupplierIdentification, string fileXml)
        {
            try
            {
                //Serialize Xml
                string xmlDecode = StringUtilies.Base64Decode(fileXml);

                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlDecode);

                if (document.DocumentElement.Name == "ApplicationResponse")
                {
                    var serializeEvent = _documentBuild.SerializeApplicationResponse(xmlDecode);

                    return FileName.BuildNameFileDian(prefix, SupplierIdentification, serializeEvent.IssueDate.Value, serializeEvent.ID.Value);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
