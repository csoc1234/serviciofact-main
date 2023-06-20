using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contributors.Models
{
    public class BaseResponse
    {
        public int Code { get; set; }

        public string Message { get; set; }
    }
}
