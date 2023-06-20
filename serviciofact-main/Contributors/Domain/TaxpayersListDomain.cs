using Contributors.Application.Dto.Response;
using Contributors.Domain.Entities;
using Contributors.Domain.Interface;
using Contributors.Infraestructure.Data.Context.Interface;
using System.Collections.Generic;

namespace Contributors.Domain
{
    public class TaxpayersListDomain : ITaxpayersListDomain
    {
        private readonly IApplicationDbContext _context;

        public TaxpayersListDomain(IApplicationDbContext context)
        {
            _context = context;
        }

        public TaxPayerListStatusResponse GetList(int status)
        {
            TaxPayerListStatusResponse response = new TaxPayerListStatusResponse();

            List<Taxpayers> result = _context.TaxPayerListEnableDian(status);

            if (result != null)
            {
                response.Code = 200;
                response.Message = "Se retorna un listado";
                response.Taxpayers = result;
            }
            else if (result == null)
            {
                response.Code = 500;
                response.Message = "No se encontraron registros";
                response.Taxpayers = null;
            }

            return response;
        }
    }
}
