using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aksharapura.DataFormatter
{
    public class Dataset
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("source")]
        public List<DataItem> SourceItems { get; set; }

        [JsonProperty("translation")]
        public List<DataItem> TranslationItems { get; set; }
    }

    public class DataItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
