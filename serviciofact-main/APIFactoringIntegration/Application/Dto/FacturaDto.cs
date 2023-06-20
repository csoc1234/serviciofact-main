namespace APIFactoringIntegration.Application.Dto
{
    public class FacturaDto
    {
        public string Xml { get; set; }

        public string NumeroDocumento { get; set; }

        public string Cufe { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime FechaPago { get; set; }

        public List<string> Eventos { get; set; }

        public decimal MontoTotal { get; set; }
    }
}
