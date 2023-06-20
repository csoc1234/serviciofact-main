using DocumentBuildCO.DocumentClass;
using System;

namespace FeCoEventos.Clients.SendEmail
{
    public class ParamSendEmail
    {
        public string NameTemplate { get; set; }
        public string SupplierId { get; set; }
        public string NameSupplier { get; set; }
        public string NameCustomer { get; set; }
        public string DocumentId { get; set; }
        public string EventId { get; set; }
        public string EventType { get; set; }
        public string EventName { get; set; }
        public string UrlWeb { get; set; }
        public string Email { get; set; }
        public string BusinessLine { get; set; }

        public ParamSendEmail(BaseDocumentClass input, string nameTemplate, string urlPortal, string eventName, string eventId, string eventType, string emailDefault = null)
        {
            this.EventName = eventName;
            this.EventId = eventId;
            this.EventType = eventType;
            this.UrlWeb = urlPortal;
            this.NameTemplate = nameTemplate;
            this.DocumentId = !String.IsNullOrEmpty(input.ID) ? input.ID : "";

            if (input.AccountingCustomerParty != null)
                this.NameCustomer = !String.IsNullOrEmpty(input.AccountingCustomerParty.RegistrationName) ?
                                 input.AccountingCustomerParty.RegistrationName : "";
            else
                this.NameCustomer = "";

            if (input.AccountingSupplierParty != null)
            {
                this.SupplierId = !String.IsNullOrEmpty(input.AccountingSupplierParty.PartyIdentificationID) ?
                                 input.AccountingSupplierParty.PartyIdentificationID : "";
                this.NameSupplier = !String.IsNullOrEmpty(input.AccountingSupplierParty.RegistrationName) ?
                                 input.AccountingSupplierParty.RegistrationName : "";
                this.Email = !String.IsNullOrEmpty(input.AccountingSupplierParty.PartyContactElectronicMail) ?
                                 input.AccountingSupplierParty.PartyContactElectronicMail : !String.IsNullOrEmpty(emailDefault) ? emailDefault : "";
            }
            else
            {
                this.SupplierId = "";
                this.NameSupplier = "";
                this.Email = !String.IsNullOrEmpty(emailDefault) ? emailDefault : "";
            }
        }
    }
}


