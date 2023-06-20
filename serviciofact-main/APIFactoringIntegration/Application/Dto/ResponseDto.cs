namespace APIFactoringIntegration.Application.Dto
{
    public class ResponseDto
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }

        public List<FacturaDto> Facturas { get; set; }
    }

    public class DataDto
    {
        public List<FacturaDto> Data { get; set; }
    }
}
