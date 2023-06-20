using Contributors.Domain.Interface;
using Contributors.Infraestructure.Data.Context.Interface;
using Contributors.Models.Response;

namespace Contributors.Domain
{
    public class StartEnableDianDomain : IStartEnableDianDomain
    {
        private readonly IApplicationDbContext _context;

        public StartEnableDianDomain(IApplicationDbContext context)
        {
            _context = context;
        }

        public ResponseBase Register(string companyId, string testSetId)
        {
            ResponseBase response = _context.RegisterTaxpayerForEnableDian(companyId, testSetId);

            if (response.Code == 200)
            {
                response.Message = "Solicitud de habilitacion registrada con exito";
            }
            else if (response.Code == 421)
            {
                response.Message = "El numero de identificacion no esta registrado";
            }
            else if (response.Code == 422)
            {
                response.Message = "El numero de identificacion ya tiene una solicitud de habilitacion";
            }
            else
            {
                response.Message = "Ha ocurrido un error durante el registro de habilitacion";
            }

            return response;
        }
    }
}
