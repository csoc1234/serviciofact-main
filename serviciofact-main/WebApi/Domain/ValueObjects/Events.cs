namespace WebApi.Domain.ValueObjects
{
    public class Events
    {
        public static string GetDescription(string responseCode)
        {
            return responseCode switch
            {
                "030" => "Acuse de recibo de Factura Electrónica de Venta",
                "031" => "Reclamo de la Factura Electrónica de Venta",
                "032" => "Recibo del bien y/o prestación del servicio",
                "033" => "Aceptación expresa",
                "034" => "Aceptación Tácita",
                "035" => "Aval",
                "036" => "Inscripción de la factura electrónica de venta como título valor - RADIAN",
                "037" => "Endoso en Propiedad",
                "038" => "Endoso en Garantía",
                "039" => "Endoso en Procuración",
                "040" => "Cancelación de endoso",
                "041" => "Limitaciones a la circulación de la factura electrónica de venta como título",
                "042" => "Terminación de las limitaciones a la circulación de la factura electrónica de venta como título",
                "043" => "Mandato",
                "044" => "Terminacion del Mandato",
                "045" => "Pago de la factura electrónica de venta como título valor",
                "046" => "Informe para el pago",
                _ => "",
            };
        }
    }
}
