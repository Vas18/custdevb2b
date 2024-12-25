
namespace custdev.domain.models
{
    public class Prompt
    {

        public string id { get; set; }
        public string Key { get; set; }
        public string PromptKey { get; set; }
        public string Language { get; set; }
        public string Model { get; set; }
        public double Temperature { get; set; }
        public string SystemMessage { get; set; }
        public string UserMessage { get; set; }
        public string Image { get; set; }
        public string JsonForm { get; set; }
        public string Function { get; set; }
        public int MaxTokens { get; set; }
        public string ResponseFormat { get; set; }
        public List<string> IncludeWords { get; set; }
        public List<string> IgnoreWords { get; set; }
    }

    public class PromptData : Prompt
    {
        public string RequestId { get; set; }
    }
}
