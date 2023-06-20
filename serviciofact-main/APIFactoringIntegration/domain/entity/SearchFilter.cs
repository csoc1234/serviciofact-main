namespace APIFactoringIntegration.Domain.Entity
{
    public class SearchFilter
    {
        public string SupplierIdentificationType { get; set; }
        public string SupplierIdentification { get; set; }
        public string CustomerIdentificationType { get; set; }
        public string CustomerIdentification { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
