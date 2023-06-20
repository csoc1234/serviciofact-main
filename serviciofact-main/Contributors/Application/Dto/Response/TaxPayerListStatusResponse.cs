using Contributors.Domain.Entities;
using Contributors.Models.Response;
using System.Collections.Generic;

namespace Contributors.Application.Dto.Response
{
    public class TaxPayerListStatusResponse : ResponseBase
    {
        public List<Taxpayers> Taxpayers { get; set; }
    }
}
