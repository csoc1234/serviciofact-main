using APIGetValidDocs.Domain.Entity;
using APIGetValidDocs.Domain.Interface;

namespace APIGetValidDocs.Domain.Core
{
    public class AccumulatorsMetric : IAccumulatorsMetric
    {
        private List<Invoice> Result = new List<Invoice>();

        public void AddResult(Invoice value)
        {
            this.Result.Add(value);
        }

        public List<Invoice> GetResult()
        {
            return this.Result;
        }

        public void Clean()
        {
            this.Result = new List<Invoice>();
        }
    }
}
