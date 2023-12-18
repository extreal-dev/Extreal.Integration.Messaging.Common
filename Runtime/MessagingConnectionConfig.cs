using System;

namespace Extreal.Integration.Messaging.Common
{
    /// <summary>
    /// Class that holds connection config for messaging.
    /// </summary>
    public class MessagingConnectionConfig
    {
        /// <summary>
        /// Group name.
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Max capacity.
        /// </summary>
        public int MaxCapacity { get; }

        /// <summary>
        /// Create a new MessagingConnectionConfig.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <param name="maxCapacity">Max capacity.</param>
        /// <exception cref="ArgumentNullException">When groupName if null.</exception>
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
