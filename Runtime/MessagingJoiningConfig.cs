using System;

namespace Extreal.Integration.Messaging.Common
{
    /// <summary>
    /// Class that holds connection config for messaging.
    /// </summary>
    public class MessagingJoiningConfig
    {
        /// <summary>
        /// Group name.
        /// </summary>
        public string GroupName { get; }


        /// <summary>
        /// Create a new MessagingConnectionConfig.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        /// <exception cref="ArgumentNullException">When groupName if null.</exception>
        public MessagingJoiningConfig(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentNullException(nameof(groupName));
            }

            GroupName = groupName;
        }
    }
}
