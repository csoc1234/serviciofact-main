using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIAttachedDocument.Infrastructure.Signed
{
    public class SignedResponse
    {
		public String code { get; set; }

		public String message { get; set; }

		public String xml { get; set; }

		public String uuid { get; set; }

		public string ToJson() => JsonConvert.SerializeObject(this);

		public static SignedResponse FromJson(string data) => JsonConvert.DeserializeObject<SignedResponse>(data);
	}
	public class SignedInternalResponse
    {
		public int Code { get; set; }
		public string File { get; set; }
		public string Message { get; set; }
    }
}
