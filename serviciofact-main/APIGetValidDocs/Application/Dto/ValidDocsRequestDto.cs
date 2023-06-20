using System.Reflection.Metadata;

namespace APIGetValidDocs.Application.Dto
{
    public class ValidDocsRequestDto
    {
        public string SupplierIdentificationType { get; set; }
        public string SupplierIdentification { get; set; }
        public string CustomerIdentificationType { get; set; }
        public string CustomerIdentification { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
