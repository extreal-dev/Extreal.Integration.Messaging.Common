using System;
using NUnit.Framework;

namespace Extreal.Integration.Messaging.Common.Test
{
    public class MessagingConnectionConfigTest
    {
        [Test]
        public void MessagingConnectionConfigWithGroupNameNull()
            => Assert.That(() => _ = new MessagingConnectionConfig(null),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.Contains("groupName"));
    }
}
