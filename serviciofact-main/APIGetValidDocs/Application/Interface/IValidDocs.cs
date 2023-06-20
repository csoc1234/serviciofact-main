using APIGetValidDocs.Application.Dto;

namespace APIGetValidDocs.Application.Interface
{
    public interface IValidDocs
    {
        Task<ResponseDto> GetListValidDocs(ValidDocsRequestDto request);

    }
}
