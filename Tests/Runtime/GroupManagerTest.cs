using System;
using NUnit.Framework;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Extreal.Integration.Messaging.Common.Test
{
    public class GroupManagerTest
    {
        private GroupManager groupManager;
        private MessagingTransportMock messagingTransportMock = new MessagingTransportMock();

        [Test]
        public void SetTransport()
        {
            groupManager.SetTransport(messagingTransportMock);
        }

        [Test]
        public void SetTransportNull()
            => Assert.That(() => groupManager.SetTransport(null),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.Contain("transport"));

        [Test]
        public async UniTask ListGroupsAsync()
        {
            // Arrange
            var expectedGroups = new List<Group> { new("testId1", "testGroupName1"), new("testId2", "testGroupName2") };

            // Act
            var actualGroups = await groupManager.ListGroupsAsync();

            // Assert
            CollectionAssert.AreEqual(expectedGroups, actualGroups, "The elements within the list should match exactly.");
        }

        [Test]
        public async UniTask DeleteGroupsAsync()
        {
            // Act
            await groupManager.DeleteGroupAsync();

            // Assert
            Assert.Pass("No return value to assert, completion of task is sufficient.");
        }
    }
}