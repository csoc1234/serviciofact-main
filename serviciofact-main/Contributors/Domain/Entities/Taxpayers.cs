using System;

namespace Contributors.Domain.Entities
{
    public class Taxpayers
    {
        public int Id { get; set; }
        public int IdEnterprise { get; set; }
        public string DocumentType { get; set; }
        public string CompanyId { get; set; }
        public string VerificationDigit { get; set; }
        public string RegistrationName { get; set; }
        public bool Active { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Environment { get; set; }
    }
}
