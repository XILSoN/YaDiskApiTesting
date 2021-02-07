namespace YaDiskApiTesting.Dto
{
    using Newtonsoft.Json;

    public class ErrorModel
    {
        public string Message { get; set; }

        public string Description { get; set; }

        public string Error { get; set; }
    }
}
