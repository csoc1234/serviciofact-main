using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeCoEventos.Clients.Signed
{
    public class SignedRequest
    {
        public String nitCertificado { get; set; }

        public String xml { get; set; }

        public String uuid { get; set; }
        public byte? typeName { get; set; }
        public string nameFile { get; set; }
    }
}
