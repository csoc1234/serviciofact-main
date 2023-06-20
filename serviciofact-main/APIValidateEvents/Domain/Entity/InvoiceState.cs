namespace APIValidateEvents.Domain.Entity
{
    public class InvoiceState
    {
        public bool Valid { get; set; }

        public List<string> EventCode { get; set; }

    }
}
