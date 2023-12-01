namespace Extreal.Integration.Messaging.Common
{
    public class MessagingConfig
    {
        public string Url { get; }

        public MessagingConfig(string url)
            => Url = url;
    }
}
