using System.Collections.Generic;
using Newtonsoft.Json;

namespace TachographReader.Application.Dtos
{
    public class DatatablesQueryModel <T>
    {
        [JsonProperty("recordsTotal")]
        public int RecordsTotal { get; set; }
        [JsonProperty("recordsFilterd")]
        public int RecordsFilterd { get; set; }
        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }
    }
}
