using Contributors.Domain.Entities;
using Contributors.Models;
using Contributors.Models.Response;
using System.Collections.Generic;

namespace Contributors.Infraestructure.Data.Context.Interface
{
    public interface IApplicationDbContext
    {
        bool ChangeStatusIssuers(int idIssuer, StatusTask process);

        ResponseBase RegisterTaxpayerForEnableDian(string companyId, string testSetId);

        List<Taxpayers> TaxPayerListEnableDian(int status);
    }
}
