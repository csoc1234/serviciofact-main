using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIGetValidDocs.Infraestructure.Database
{
    public class TInvoiceFactoring
    {
        public int Id { get; set; }
        public string PathFileXML { get; set; }
        public string DocumentId { get; set; }
        public string InvoiceUuid { get; set; }
        public string InvoiceUuidType { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string SupplierTypeIdentification { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string SupplierIdentification { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string CustomerTypeIdentification { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string CustomerIdentification { get; set; }
        public Boolean Active { get; set; }
        public Int16 Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime InvoiceIssuedate { get; set; }
        public int IdEnterprise { get; set; }
        public int InvoiceId { get; set; }
        public DateTime PaymentDate { get; set; }

        public decimal PayableAmount { get; set; }
    }
}
