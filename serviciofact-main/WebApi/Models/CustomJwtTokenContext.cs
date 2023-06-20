using Newtonsoft.Json;

namespace WebApi.Models
{
    public class CustomJwtTokenContext
    {
        public class UserClass
        {
            [JsonProperty("enterpriseNit", Required = Required.Always)]
            public string EnterpriseNit { get; set; }

            [JsonProperty("enterpriseToken", Required = Required.Always)]
            public string EnterpriseToken { get; set; }

            [JsonProperty("entepriseId", Required = Required.Always)]
            public string EnterpiseId { get; set; }

            [JsonProperty("enterpriseschemeid", Required = Required.Always)]
            public string EnterpiseSchemeId { get; set; }

            public int GetEnterpiseIdInt()
            {
                int.TryParse(this.EnterpiseId, out int result);

                return result;
            }
        }

        [JsonProperty("user", Required = Required.Always)]
        public UserClass User { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static CustomJwtTokenContext FromJson(string data)
        {
            return JsonConvert.DeserializeObject<CustomJwtTokenContext>(data);
        }

    }
}
