using FeCoEventos.Models.Responses;
using System;

namespace FeCoEventos.Domain.Entity
{
    public class Certificate
    {
        public int Id { get; set; }

        public int IdEnterprise { get; set; }

        public string Serie { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string Identification { get; set; }

        public bool Active { get; set; }

        public string FriendlyName { get; set; }

        public string ProviderName { get; set; }

        public string NameFile { get; set; }

        public byte? TypeName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }        
    }
}
