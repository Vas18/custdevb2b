
using custdev.domain.models;

namespace custdev.domain.interfaces
{
    public interface IAiService
    {
        Task<(PromptData, dynamic)> GetAiResponse(PromptData prompt);
        Task<string> GetAudioTranscriptionAsync(string audioUrl);
    }
}
