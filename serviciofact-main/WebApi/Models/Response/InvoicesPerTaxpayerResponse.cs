using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Response
{
    public class InvoicesPerTaxpayerResponse : ResponseBase
    {
        public List<InvoicesPerTaxpayerList> InvoicesList { get; set; }
        public partial class InvoicesPerTaxpayerList
        {
            public int Id { get; set; }
            public string DocumentId { get; set; }
            public string UUID { get; set; }
            public string trackId { get; set; }
            public string pathFile { get; set; }
            public string nameFile { get; set; }
        }
    }
}
