using System;

namespace WebApi.Infrastructure.Data.Context
{
    public class InvoiceFactoringTable
    {
        public int id { get; set; }
        public int id_enterprise { get; set; }
        public int invoice_id { get; set; }
        public string document_id { get; set; }
        public string invoice_uuid { get; set; }
        public string invoice_uuid_type { get; set; }
        public DateTime invoice_issuedate { get; set; }
        public string supplier_type_identification { get; set; }
        public string supplier_identification { get; set; }
        public string customer_type_identification { get; set; }
        public string customer_identification { get; set; }
        public bool active { get; set; }
        public Int16 status { get; set; }
        public string path_file_xml { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public DateTime payment_date { get; set; }

        public decimal payable_amount { get; set; }
    }
}