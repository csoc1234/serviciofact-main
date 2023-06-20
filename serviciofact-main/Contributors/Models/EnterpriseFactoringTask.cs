using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contributors.Models
{
    public class EnterpriseFactoringTask
    {
        public int Id { get; set; }

        public int IdEnterpriseFactoring { get; set; }

        public int Process { get; set; }

        public string ProcessName { get; set; }

        public int Status { get; set; }
    }
}
