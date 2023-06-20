using System;

namespace Contributors.Models
{
  public class EnterpriseTable
  {
    public int id { get; set; }
    public int id_enterprise { get; set; }
    public string document_type { get; set; }
    public string company_id { get; set; }
    public string verification_digit { get; set; }
    public string registration_name { get; set; }
    public bool active { get; set; }
    public int status { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public int environment { get; set; }
  }
}