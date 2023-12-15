using System;

namespace Extreal.Integration.Messaging.Common
{
    public class MessagingConnectionConfig
    {
        public string GroupName { get; }
        public int MaxCapacity { get; }

        public MessagingConnectionConfig(string groupName, int maxCapacity = default)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentNullException(nameof(groupName));
            }

            GroupName = groupName;
            MaxCapacity = maxCapacity;
        }
    }
}
