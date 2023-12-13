namespace Extreal.Integration.Messaging.Common
{
    public class MessagingConnectionConfig
    {
        public string GroupName { get; }
        public int MaxCapacity { get; }

        public MessagingConnectionConfig(string groupName, int maxCapacity = default)
        {
            GroupName = groupName;
            MaxCapacity = maxCapacity;
        }
    }
}
