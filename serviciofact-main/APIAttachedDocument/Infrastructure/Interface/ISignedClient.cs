using APIAttachedDocument.Domain.Entity;
using APIAttachedDocument.Infrastructure.Logging;
using APIAttachedDocument.Infrastructure.Signed;

namespace APIAttachedDocument.Infrastructure.Interface
{
    public interface ISignedClient
    {
        SignedInternalResponse SignedXml(string xml, Certificate certificate, ILogAzure log);
    }
}
