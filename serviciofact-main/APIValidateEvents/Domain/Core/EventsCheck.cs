using APIValidateEvents.Domain.Entity;

namespace APIValidateEvents.Domain.Core
{
    public class EventsCheck
    {

        public static InvoiceState Check(InvoiceStatusDian invoiceStatus)
        {
            if (invoiceStatus.CantidadEventos > 0)
            {
                //Eventos descartables 
                //Reclamo 031
                //Endoso 037, 038, 039,
                if (invoiceStatus.EsReclamada || invoiceStatus.EstaEndosada)
                {
                    return new InvoiceState { Valid = false };
                }

                //Tiene 030 y 032
                if (invoiceStatus.EsRecibida && invoiceStatus.EsBienoServicio)
                {
                    //Si tiene 030 032 y alguno de 033 o 034, pero no tiene mas ninguno
                    if (invoiceStatus.Events.All(x => x.ResponseCode == "030" || x.ResponseCode == "032"))
                    {
                        //Leer evento 032
                        EventsDocumentResponse? event032 = invoiceStatus.Events.Where(x => x.ResponseCode == "032").FirstOrDefault();
                        DateTime dateTimeNow = DateTime.UtcNow.AddHours(-5);
                        //Si la fecha del evento es de -3 dias continuos atras
                        if (dateTimeNow.Subtract(event032.EffectiveDate).Days > 3)
                        {
                            //Valido
                            return new InvoiceState { Valid = true, EventCode = invoiceStatus.Events.Select(x => x.ResponseCode).ToList() };
                        }
                    }

                    //Tiene 033 o 034
                    if (invoiceStatus.EsAceptada || invoiceStatus.EsAceptadaTacitamente)
                    {
                        List<string> listCodeNotAllowed = new List<String> { "035", "036", "040", "041", "042", "043", "044", "045", "046" };

                        //Si tiene 030 032 y alguno de 033 o 034, pero no tiene mas ninguno
                        if (!invoiceStatus.Events.Where(x => listCodeNotAllowed.Contains(x.ResponseCode)).Any())
                        {
                            //Valido
                            return new InvoiceState { Valid = true, EventCode = invoiceStatus.Events.Select(x => x.ResponseCode).ToList() };
                        }
                    }
                }
                else
                {
                    return new InvoiceState { Valid = false };
                }
            }
            else
            {
                return new InvoiceState { Valid = false };
            }

            return new InvoiceState { Valid = false };
        }
    }
}
