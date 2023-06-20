using DocumentBuildCO.DocumentClass.UBL2._1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Response
{
    public class ValidateXmlResponse
    {
        public bool Valid { get; set; }

        public string Message { get; set; }

        public BaseDocument21 Document { get; set; }

        public DateTime ValidateDianDatetime { get; set; }
    }
}
