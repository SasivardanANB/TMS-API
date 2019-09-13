using TMS.DomainObjects.Objects;

namespace TMS.DomainObjects.Request
{
    public class FCMRequest
    {
        public FCMNotification notification { get; set; }
        public string to { get; set; }
    }
}
