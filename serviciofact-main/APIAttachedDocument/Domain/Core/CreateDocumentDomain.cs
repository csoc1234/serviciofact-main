using APIAttachedDocument.Domain.Entity;
using APIAttachedDocument.Domain.Interface;
using APIAttachedDocument.Infrastructure;
using APIAttachedDocument.Infrastructure.Interface;
using APIAttachedDocument.Infrastructure.Logging;
using APIAttachedDocument.Transversal;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace APIAttachedDocument.Domain.Core
{
    public class CreateDocumentDomain : ICreateDocumentDomain
    {
        private readonly ICertificateClient _certificateClient;
        private readonly ISignedClient _signedClient;

        public CreateDocumentDomain(ICertificateClient certificateClient, ISignedClient signedClient)
        {
            _certificateClient = certificateClient;
            _signedClient = signedClient;
        }

        public Entity.AttachedDocument Generate(string xmlEvent, string xmlDian, EnterpriseCredential enterpriseCredential, ILogAzure log)
        {
            Stopwatch timeT = new Stopwatch();
            timeT.Start();

            try
            {
                //Serializamos el xml ApplicationResponse del Evento
                DocumentElectronic? document = BuildDocument.SerializeApplicationResponse(StringUtilies.Base64Decode(xmlEvent));

                if (document == null)
                {
                    return new Entity.AttachedDocument
                    {
                        Code = 500,
                        Message = "El archivo xml del evento no es posible leerlo, posiblemente no cuente la estructura esperada UBL 2.1 ApplicationResponse"
                    };
                }

                DocumentElectronic? applicationResponseTypeDian = BuildDocument.SerializeApplicationResponse(StringUtilies.Base64Decode(xmlDian));

                if (applicationResponseTypeDian == null)
                {
                    return new Entity.AttachedDocument
                    {
                        Code = 500,
                        Message = "El archivo xml de la DIAN no es posible leerlo, posiblemente no cuente la estructura esperada UBL 2.1 ApplicationResponse"
                    };
                }

                //Validar que este aprobado por la DIAN
                if (applicationResponseTypeDian.ValidationCode != "02")
                {
                    return new Entity.AttachedDocument
                    {
                        Code = 99,
                        Message = "El Evento de Aceptacion DIAN no es de tipo 02 Documento validado por la DIAN"
                    };
                }

                if (document.Uuid != applicationResponseTypeDian.ReferenceUuid)
                {
                    return new Entity.AttachedDocument
                    {
                        Code = 103,
                        Message = "El CUDE referencia del Evento de Aceptacion DIAN no es igual al CUDE del Evento"
                    };
                }

                if (document.DocumentId != applicationResponseTypeDian.ReferenceId)
                {
                    return new Entity.AttachedDocument
                    {
                        Code = 103,
                        Message = "El ID referencia del Evento de Aceptacion DIAN no es igual al ID del Evento"
                    };
                }

                string? xmlFile = BuildDocument.BuildAttachedDocument(xmlEvent, xmlDian, document, applicationResponseTypeDian);

                if (!string.IsNullOrEmpty(xmlFile))
                {
                    //Obtener Certificado

                    CertificateResponse resultCertificate = new CertificateResponse();

                    if (enterpriseCredential.Type == 1)
                    {
                        resultCertificate = _certificateClient.GetCertificate(enterpriseCredential.TokenJwt, log);

                        if (resultCertificate.Code != 200)
                        {
                            return new Entity.AttachedDocument
                            {
                                Code = resultCertificate.Code,
                                Message = resultCertificate.Message
                            };
                        }
                    }
                    else if (enterpriseCredential.Type == 2)
                    {
                        resultCertificate = _certificateClient.GetCertificateByIdentification(enterpriseCredential.IdEnterprise, enterpriseCredential.Identification, log);

                        if (resultCertificate.Code != 200)
                        {
                            return new Entity.AttachedDocument
                            {
                                Code = resultCertificate.Code,
                                Message = resultCertificate.Message
                            };
                        }
                    }

                    //Firmar Xml
                    Infrastructure.Signed.SignedInternalResponse? resultSigned = _signedClient.SignedXml(xmlFile, resultCertificate.Certificate, log);

                    //Si la firma no es exitosa, termino la transaccion
                    if (resultSigned.Code != 200)
                    {
                        return new Entity.AttachedDocument
                        {
                            Code = resultSigned.Code,
                            Message = resultSigned.Message
                        };
                    }

                    AttachedDocument? result = new Entity.AttachedDocument
                    {
                        Code = 200,
                        Message = "Se ha generado con exito el archivo contenedor"
                    };

                    timeT.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, "Se ha generado con exito el archivo contenedor", LevelMsn.Warning, timeT.ElapsedMilliseconds);




                    return new Entity.AttachedDocument
                    {
                        Code = result.Code,
                        Message = result.Message,
                        File = resultSigned.File,
                        Namefile = FileNameXml.GetName(document),
                        Hash = StringUtilies.GetSHA1(resultSigned.File),
                        Size = Convert.FromBase64String(resultSigned.File).Length,
                    };
                }
                else
                {
                    AttachedDocument? result = new Entity.AttachedDocument
                    {
                        Code = 500,
                        Message = "No se ha logrado generar el archivo xml contenedor"
                    };

                    timeT.Stop();
                    log.WriteComment(MethodBase.GetCurrentMethod().Name, result.Message, LevelMsn.Warning, timeT.ElapsedMilliseconds);

                    return new Entity.AttachedDocument
                    {
                        Code = result.Code,
                        Message = result.Message
                    };
                }
            }
            catch (Exception ex)
            {
                timeT.Stop();
                log.WriteComment(MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(ex), LevelMsn.Error, timeT.ElapsedMilliseconds);

                return new Entity.AttachedDocument
                {
                    Code = 500,
                    Message = "No se ha logrado generar el archivo xml contenedor"
                };
            }
        }
    }
}
