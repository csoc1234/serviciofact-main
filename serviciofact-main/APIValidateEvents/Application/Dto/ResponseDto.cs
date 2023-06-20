using APIValidateEvents.Common;

namespace APIValidateEvents.Application.Dto
{
    public class ResponseDto : ResponseBase
    {
        public bool Valid { get; set; }

        public List<string> EventCode { get; set; }
    }
}
