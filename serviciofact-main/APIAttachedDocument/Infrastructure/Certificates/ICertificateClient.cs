using APIAttachedDocument.Infrastructure.Logging;

namespace APIAttachedDocument.Infrastructure
{
    public interface ICertificateClient
    {
        CertificateResponse GetCertificate(string tokenJWT, ILogAzure log);

        CertificateResponse GetCertificateByIdentification(string idEnterprise, string identification, ILogAzure log);
    }
}
