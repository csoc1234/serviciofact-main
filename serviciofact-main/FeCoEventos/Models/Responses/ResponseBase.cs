using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeCoEventos.Models.Responses
{
    public class ResponseBase
    {
        public int Code { get; set; }

        public string Message { get; set; }
    }
}
