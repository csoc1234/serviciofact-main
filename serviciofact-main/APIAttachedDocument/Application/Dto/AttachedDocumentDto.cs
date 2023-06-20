using APIAttachedDocument.Transversal.Model;

namespace APIAttachedDocument.Application.Dto
{
    public class AttachedDocumentDto : ResponseBase
    {
        public string Namefile { get; set; }

        public string File { get; set; }

        public string Hash { get; set; }

        public int Size { get; set; }
    }
}
