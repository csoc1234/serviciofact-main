using APIAttachedDocument.Transversal.Model;

namespace APIAttachedDocument.Domain.Entity
{
    public class AttachedDocument : ResponseBase
    {
        public string Namefile { get; set; }

        public string File { get; set; }

        public string Hash { get; set; }

        public int Size { get; set; }
    }
}
