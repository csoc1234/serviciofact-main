using FeCoEventos.Util;
using System;

namespace FeCoEventos.Domain.ValueObjects
{
    public class FileName
    {
        public static string BuildNameFileDian(string prefix, string supplierIdentification, DateTime issueDate, string consecutivo)
        {
            //Auto completado de numeracion del Nit
            int countNit = supplierIdentification.Length;
            string cerosExtrasNit = "";
            for (int i = countNit; i < 10; i++)
            {
                cerosExtrasNit = cerosExtrasNit + "0";
            }
            supplierIdentification = cerosExtrasNit + supplierIdentification;

            var codidoDian = "016";

            //Auto completado de numeracion de consecutivo

            int count = consecutivo.Length;
            string cerosExtras = "";
            for (int i = count; i < 8; i++)
            {
                cerosExtras = cerosExtras + "0";
            }
            consecutivo = cerosExtras + consecutivo;

            return string.Format("{0}{1}{2}{3}{4}.xml", prefix, supplierIdentification, codidoDian, issueDate.ToString("yy"), consecutivo);
        }
    }
}
