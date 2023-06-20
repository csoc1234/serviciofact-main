using FeCoEventos.Models.Responses;

namespace FeCoEventos.Application.Dto
{
    public class FileDto : ResponseBase
    {
        public FileXml FileXml { get; set; }
    }

    public class FileXml
    {
        public string File { get; set; }

        public string Name { get; set; }
    }
}
