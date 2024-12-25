
namespace custdev.domain.configuration
{
    public class PromptConfig
    {
        public string PromptKey { get; set; }
        public string Model { get; set; }
        public string PromptSystem { get; set; }
        public string PromptUser { get; set; }
        public string PromptJson { get; set; }
        public string ResponseFormat { get; set; }
        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
    }
}
