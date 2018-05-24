using Newtonsoft.Json;
using System.Collections.Generic;

namespace RequestApprovalTestApp.Utils
{
    public class ODataResponse<T>
    {
        [JsonProperty("value")]
        public IEnumerable<T> Values { get; set; }
    }
}