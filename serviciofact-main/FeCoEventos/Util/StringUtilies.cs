using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace FeCoEventos.Util
{
    public class StringUtilies
    {
        public static string GetSHA1(string str)
        {
            SHA1 sha1 = SHA1CryptoServiceProvider.Create();
            Byte[] textOriginal = ASCIIEncoding.Default.GetBytes(str);
            Byte[] hash = sha1.ComputeHash(textOriginal);
            StringBuilder cadena = new StringBuilder();
            foreach (byte i in hash)
            {
                cadena.AppendFormat("{0:x2}", i);
            }
            return cadena.ToString();
        }

        public static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes;
            try
            {
                base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);

                string returnValue = System.Text.UTF8Encoding.UTF8.GetString(base64EncodedBytes);

                #region Limpieza de caracteres inválidos al inicio del XML
                returnValue = DocumentBuildCO.Common.Utils.CleanInvalidCharactersAtBegining(returnValue); //Tomamos el archivo desde el primer caracter válido
                #endregion

                return returnValue;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string Base64Encode(string plainText)
        {
            byte[]? plainTextBytes = new byte[1];
            try
            {
                plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            }
            catch (Exception)
            {
            }
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string BOMStringXml(string xml)
        {
            XmlDocument doc = new XmlDocument
            {
                PreserveWhitespace = true
            };

            xml = StringUtilies.Base64Decode(xml);

            try
            {
                doc.LoadXml(xml);

                return StringUtilies.Base64Encode(xml);
            }
            catch
            {
                try
                {
                    string BOMMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (xml.StartsWith(BOMMarkUtf8))
                    {
                        xml = xml.Remove(0, BOMMarkUtf8.Length);
                    }
                    doc.LoadXml(xml);
                    return StringUtilies.Base64Encode(xml);
                }
                catch (Exception)
                {
                    try
                    {
                        doc.LoadXml(xml.Substring(xml.IndexOf(Environment.NewLine)));
                        return StringUtilies.Base64Encode(xml.Substring(xml.IndexOf(Environment.NewLine)));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }

        public static string FillWithSpace(string value, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "";
            }

            int i = length - value.Length;

            if (i > 0)
            {
                for (int j = value.Length; j <= length; j++)
                {
                    value = value + " ";
                }
            }

            return value;
        }
    }
}
