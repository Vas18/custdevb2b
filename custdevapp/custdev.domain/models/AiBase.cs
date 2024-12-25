
using Newtonsoft.Json;

namespace custdev.domain.models
{
    public class AiBase
    {
        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }
    }
}
