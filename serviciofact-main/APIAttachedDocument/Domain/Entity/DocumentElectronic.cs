namespace APIAttachedDocument.Domain.Entity
{
    public class DocumentElectronic
    {
        public string Uuid { get; set; }

        public string UuidSchemeName { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime IssueTime { get; set; }

        public string DocumentId { get; set; }

        public string ProfileExecutionID { get; set; }

        public DocumentParty SenderParty { get; set; }

        public DocumentParty ReceiverParty { get; set; }

        public string ValidationCode { get; set; }

        public string ReferenceUuid { get; set; }

        public string ReferenceId { get; set; }
    }

    public class DocumentParty
    {
        public string RegistrationName { get; set; }

        public string CompanyID { get; set; }

        public string CompanyIDSchemeID { get; set; }

        public string CompanyIDSchemeName { get; set; }

        public string TaxSchemeID { get; set; }

        public string TaxSchemeName { get; set; }

    }
}

