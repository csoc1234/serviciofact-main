using System;

namespace TFHKA.EventsDian.Infrastructure.Data.Context
{
    public class InvoiceEventTable
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
        public string event_type { get; set; }
        public string event_id { get; set; }
        public string event_uuid { get; set; }
        public string event_uuid_type { get; set; }
        public bool active { get; set; }
        public Int16 status { get; set; }
        public Int16 dian_status { get; set; }
        public string dian_message { get; set; }
        public DateTime dian_response_datetime { get; set; }
        public string path_file { get; set; }
        public string namefile { get; set; }
        public string session_log { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string track_id { get; set; }
        public string dian_result_validation { get; set; }
        public string dian_result_pathfile { get; set; }
        public string dian_result_namefile { get; set; }
        public int tries_send { get; set; }
        public string email { get; set; }
        public int environment { get; set; }
        public string attacheddocument_namefile { get; set; }
        public string attacheddocument_pathfile { get; set; }
        public byte try_query { get; set; }
        public Int16? created_by { get; set; }
    }
}