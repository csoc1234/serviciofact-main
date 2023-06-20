namespace APIFactoringIntegration.Application.Dto
{
    public class DocumentRequestDto
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string TipoDocumentoProveedor { get; set; }
        public string IdProveedor { get; set; }
        public string TipoDocumentoPagador { get; set; }
        public string IdPagador { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
    }
}
