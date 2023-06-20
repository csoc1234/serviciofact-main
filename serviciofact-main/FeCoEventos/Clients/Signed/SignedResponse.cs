using System;

namespace FeCoEventos.Clients.Signed
{
    public class SignedResponse
    {
        public String code { get; set; }

        public String message { get; set; }

        public String xml { get; set; }

        public String uuid { get; set; }
    }
    public class SignedInternalResponse
    {
        public int Code { get; set; }
        public string File { get; set; }
        public string Message { get; set; }
    }
}
