using System.Collections.Generic;
using WebApi.Domain.Entity;

namespace WebApi.Domain.Interface
{
    public interface ICheckStatusEventsDomain
    {
        Entity.InvoiceEventsStatusDian Valid(List<InvoiceStatusLast> events);
    }
}
