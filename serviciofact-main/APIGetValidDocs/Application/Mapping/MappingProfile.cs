using AutoMapper;
using APIGetValidDocs.Application.Dto;
using APIGetValidDocs.Domain.Entity;

namespace APIGetValidDocs.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Transformar Invoice to ValidDocsResponseDto
            CreateMap<Invoice, ValidDocsResponseDto>();
        }
    }
}
