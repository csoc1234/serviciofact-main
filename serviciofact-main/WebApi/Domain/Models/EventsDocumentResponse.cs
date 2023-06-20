using System;

namespace WebApi.Domain.Models
{
    public class EventsDocumentResponse
    {
        public int Status { get; set; }

        public string ReferenceID { get; set; }

        public string ResponseCode { get; set; }

        public string Description { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int ID { get; set; }

        public string EventId { get; set; }

        public string UUID { get; set; }

        public string UUIDSchemeName { get; set; }

        public Party IssuerParty { get; set; }

        public Party RecipientParty { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class Party
    {
        public string RegistrationName { get; set; }

        public string CompanyID { get; set; }

        public string CompanyIDSchemeName { get; set; }
    }
}
