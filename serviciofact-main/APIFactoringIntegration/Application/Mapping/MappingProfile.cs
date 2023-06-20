using APIFactoringIntegration.Application.Dto;
using APIFactoringIntegration.Domain.Entity;
using AutoMapper;

namespace APIFactoringIntegration.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Transformar Invoice to ValidDocsResponseDto
            CreateMap<Invoice, FacturaDto>()
                .ForMember(dest => dest.Xml, opt => opt.MapFrom(src => src.Xml))
                .ForMember(dest => dest.NumeroDocumento, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.Cufe, opt => opt.MapFrom(src => src.Cufe))
                .ForMember(dest => dest.FechaEmision, opt => opt.MapFrom(src => src.IssueDate))
                .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.Eventos, opt => opt.MapFrom(src => src.EventCode))
                .ForMember(dest => dest.MontoTotal, opt => opt.MapFrom(src => src.PayableAmount));
        }
    }
}
