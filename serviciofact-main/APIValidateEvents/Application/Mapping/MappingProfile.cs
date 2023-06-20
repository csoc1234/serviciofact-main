using APIValidateEvents.Application.Dto;
using APIValidateEvents.Domain.Entity;
using AutoMapper;

namespace APIValidateEvents.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //transform DianResponse to ResponseDianDto
            CreateMap<InvoiceState, ResponseDto>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Valid ? 200 : 422))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Valid ? "Valido" : "Factura no es candidata"))
                .ForMember(dest => dest.Valid, opt => opt.MapFrom(src => src.Valid))
                .ForMember(dest => dest.EventCode, opt => opt.MapFrom(src => src.EventCode));
        }
    }
}
