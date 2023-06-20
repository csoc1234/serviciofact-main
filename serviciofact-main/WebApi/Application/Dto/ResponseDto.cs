namespace WebApi.Application.Dto
{
    public class ResponseDto
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public InvoiceDto Invoice { get; set; }
    }
}