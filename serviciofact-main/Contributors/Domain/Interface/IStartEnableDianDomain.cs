using Contributors.Models.Response;

namespace Contributors.Domain.Interface
{
    public interface IStartEnableDianDomain
    {
        ResponseBase Register(string companyId, string testSetId);
    }
}
