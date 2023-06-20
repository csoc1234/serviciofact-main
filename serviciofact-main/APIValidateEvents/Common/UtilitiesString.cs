using System.Xml;

namespace APIValidateEvents.Common
{
    public class UtilitiesString
    {
        public static bool IsValidXML(string xml)
        {
            try
            {
                string xmlPlain = UtilitiesString.Base64Decode(xml);

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(xmlPlain);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsDateValid(string date)
        {
            //Si es vacio no valido la fecha por que es opcional
            if (string.IsNullOrEmpty(date))
            {
                return true;
            }

            try
            {
                DateTime.Parse(date);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes;
            try
            {
                base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            }
            catch (Exception)
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
