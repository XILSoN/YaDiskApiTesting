namespace YaDiskApiTesting.Dto
{
    using Newtonsoft.Json;
    using Refit;

    public class Link
    {
        [JsonProperty("operation_id")]
        public string OperationId { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("templated")]
        public bool Templated { get; set; }
    }
}
