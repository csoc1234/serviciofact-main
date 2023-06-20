using APIGetValidDocs.Domain.Entity;

namespace APIGetValidDocs.Domain.Interface
{
    public interface IAccumulatorsMetric
    {
        void AddResult(Invoice value);

        List<Invoice> GetResult();

        void Clean();
    }
}
