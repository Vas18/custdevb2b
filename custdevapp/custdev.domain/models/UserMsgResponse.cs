
namespace custdev.domain.models
{
    public class UserMsgResponse
    {
        public string PlatformId { get; set; }
        
        public string Language { get; set; }
        public string Question { get; set; }
        public string FileUrl { get; set; }
        public string Text { get; set; }
        public string ClientId { get; set; }
        
        public bool IsFinal { get; set; }
    }
}
