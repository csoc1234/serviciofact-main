using APIFactoringIntegration.Application.Dto;

namespace APIFactoringIntegration.Application.Interface
{
    public interface IDocument
    {
        Task<ResponseDto> ValidDocument(DocumentRequestDto request);
    }
}
