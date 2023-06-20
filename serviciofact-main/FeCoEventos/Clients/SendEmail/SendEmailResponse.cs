using System;

namespace FeCoEventos.Clients.SendEmail
{
    public class SendEmailResponse
    {
        public int Code { get; set; }

        public String Message { get; set; }

        public Boolean Result { get; set; }

        public int EmailRequestId { get; set; }

    }

    public class SendEmailInternalResponse
    {
        public int Code { get; set; }

        public int EmailRequestId { get; set; }

        public string Message { get; set; }
    }
}
