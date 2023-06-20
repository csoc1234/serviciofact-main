using APIAttachedDocument.Domain.Entity;

namespace APIAttachedDocument.Domain.Core
{
    public class FileNameXml
    {
        public static string GetName(DocumentElectronic applicationResponse)
        {
            string identification = String.Empty;

            identification = applicationResponse.SenderParty.CompanyID;

            string codidoDian = "016";

            string name = string.Format("ad{0}{1}{2}{3}.xml", identification, codidoDian, applicationResponse.IssueDate.ToString("yy"), applicationResponse.DocumentId);

            return name;
        }
    }
}
