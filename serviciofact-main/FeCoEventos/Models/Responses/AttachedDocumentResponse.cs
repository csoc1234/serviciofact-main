namespace FeCoEventos.Models.Responses
{
    public class AttachedDocumentResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public string Namefile { get; set; }

        public string File { get; set; }

        public string Hash { get; set; }

        public int Size { get; set; }
    }
}
