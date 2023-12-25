using System;
using NUnit.Framework;

namespace Extreal.Integration.Messaging.Common.Test
{
    public class GroupConfigTest
    {
        [Test]
        public void GroupConfigWithGroupNameNull()
            => Assert.That(() => _ = new GroupConfig(null),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.Contains("groupName"));
    }
}
