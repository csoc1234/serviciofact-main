using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Domain.Entity
{
    public class Invoice
    {
        public int Id { get; set; }
        public int IdEnterprise { get; set; }
        public string DocumentId { get; set; }
        public string Cufe { get; set; }
        public string PathFile { get; set; }
        public string NameFile { get; set; }
        public string TrackId { get; set; }
  }
}
