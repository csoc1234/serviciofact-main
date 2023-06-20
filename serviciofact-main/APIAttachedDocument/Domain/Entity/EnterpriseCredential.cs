namespace APIAttachedDocument.Domain.Entity
{
    public class EnterpriseCredential
    {
        //1 Token JWT
        //2 Datos Enterprise
        public int Type { get; set; }

        public string TokenJwt { get; set; }

        public string IdEnterprise { get; set; }

        public string Identification { get; set; }
    }
}
