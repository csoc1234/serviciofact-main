using System.Text;
using System.Security.Cryptography;

namespace APIAttachedDocument.Transversal
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
            }
            catch (Exception)
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = new byte[1];
            try
            {
                plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            }
            catch (Exception)
            {
            }
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}
