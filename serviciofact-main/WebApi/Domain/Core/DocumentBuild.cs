using DocumentBuildCO.ClassXSD;
using DocumentBuildCO.Reader.UBL2._1;
using System;
using System.Xml;

namespace WebApi.Domain.Core
{
    public class DocumentBuild
    {
        public static ApplicationResponseType ApplicationResponseProcess(string XmlApplicationResponse)
        {
            try
            {
                string xmlPlain = Invoice21Domain.Base64Decode(XmlApplicationResponse);

                #region Codigo Temporal Reemplazo de cbc:EffectiveTime - a :

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.LoadXml(xmlPlain);

                //Load Node Main
                //Namespaces
                XmlNamespaceManager nms = new XmlNamespaceManager(doc.NameTable);
                nms.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nms.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

                var applicationResponseMain = doc.GetElementsByTagName("ApplicationResponse");
                XmlNode applicationResponseNode = applicationResponseMain.Item(0);
                XmlElement applicationResponseXML = (XmlElement)applicationResponseNode;

                var documentResponseXmlList = XmlReadNode.ExtractNodeList(applicationResponseXML, "cac:DocumentResponse", nms);

                if (documentResponseXmlList != null)
                {
                    foreach (var rowNode in documentResponseXmlList)
                    {
                        //Extraer cac:Response
                        var responseNode = XmlReadNode.ExtractNode((XmlElement)rowNode, "cac:Response", nms);

                        //Extraer cbc:EffectiveTime
                        var effectiveTimeNode = XmlReadNode.ExtractNode(responseNode, "cbc:EffectiveTime", nms);

                        string timeExtract = effectiveTimeNode.InnerXml;

                        //Correccion de Tiempo
                        //Extraer tiempo
                        string timeOld = timeExtract[..8];
                        //Reemplazo - por :
                        string timeCorrected = timeOld.Replace("-", ":");

                        xmlPlain = xmlPlain.Replace(timeOld, timeCorrected);
                    }
                }

                #endregion

                ApplicationResponseType applicationResponse = null;

                xmlPlain = Invoice21Domain.Base64Encode(xmlPlain);

                applicationResponse = ApplicationResponseSerialize(xmlPlain);

                return applicationResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ApplicationResponseType ApplicationResponseSerialize(string XmlApplicationResponse)
        {
            try
            {
                string xmlPlain = Invoice21Domain.Base64Decode(XmlApplicationResponse);

                ApplicationResponseType applicationResponse = null;

                applicationResponse = DocumentBuildCO.Serialize.SerializeUBL21.ApplicationResponse(xmlPlain);

                return applicationResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
