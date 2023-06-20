using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contributors.Models
{
    public class EnterpriseFactoringHab
    {
        public int id { get; set; }

        public DateTime created_at { get; set; }

        public DateTime finished_at { get; set; }

        public int status { get; set; }

        public string company_id { get; set; }

        public int environment { get; set; }

        public string testset_id { get; set; }

    }
}
