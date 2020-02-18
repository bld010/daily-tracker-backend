using Newtonsoft.Json;

namespace Program
{
    public class DiaryItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Rating { get; set; }
        public string Note { get; set; }
        public string UserName { get; set; }
        public string TimeStamp { get; set; }
    }
}