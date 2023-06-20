using APIAttachedDocument.Application.Dto;
using APIAttachedDocument.Domain.Entity;
using APIAttachedDocument.Infrastructure.Logging;

namespace APIAttachedDocument.Application.Interface
{
    public interface ICreateDocument
    {
        AttachedDocumentDto Generate(FilesXmlDto request, EnterpriseCredential enterpriseCredential, LogRequest logRequest);
    }
}
