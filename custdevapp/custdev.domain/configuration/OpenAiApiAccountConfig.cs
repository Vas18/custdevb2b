
namespace custdev.domain.configuration
{
    public class OpenAiApiAccountConfiguration
    {
        public string Name { get; set; }
        public string OrganizationId { get; set; }
        public string SecretKey { get; set; }
    }

    public class OpenAiDefaultConfiguration : OpenAiApiAccountConfiguration
    {
    }
}
