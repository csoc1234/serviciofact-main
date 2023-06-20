using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Application.Dto
{
    public class InvoicesPerTaxpayerDto
    {
        public int IdEnterprise { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
