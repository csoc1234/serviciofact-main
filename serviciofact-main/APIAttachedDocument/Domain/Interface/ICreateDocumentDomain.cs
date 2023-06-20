using APIAttachedDocument.Domain.Entity;
using APIAttachedDocument.Infrastructure.Logging;

namespace APIAttachedDocument.Domain.Interface
{
    public interface ICreateDocumentDomain
    {
        public Entity.AttachedDocument Generate(string xmlEvent, string xmlDian, EnterpriseCredential enterpriseCredential, ILogAzure log);
    }
}
