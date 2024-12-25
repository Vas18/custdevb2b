
using custdev.domain.models;

namespace custdev.domain.db
{
    public class DbMessage : UserMsgResponse
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
