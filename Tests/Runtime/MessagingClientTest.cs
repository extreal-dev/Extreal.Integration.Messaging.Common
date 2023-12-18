using System;
using UniRx;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Extreal.Integration.Messaging.Common.Test
{
    public class MessagingClientTest
    {
        private MessagingTransportMock messagingTransportMock = new MessagingTransportMock();
        private MessagingClient messagingClient;

        private readonly List<string> connectedUsers = new List<string>();

        private string onConnectedLocalUser;

        private string onDisconnectingReason;

        private string onUnexpectedDisconnectedReason;

        private IObservable<Unit> onConnectionApprovalRejected;

        private string onUserConnectedRemoteUser;

        private (string user, string message) onMessageReceivedMessageAndUser;

        private string onUserDisconnectingUser;

        private readonly string user1 = "testUser1";
        private readonly string user2 = "testUser2";

        private IObservable<(string userId, string message)> onMessageReceived;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        [SetUp]
        public void InitializeClient()
        {
            messagingClient.SetTransport(messagingTransportMock);

            messagingClient.OnConnected.Subscribe(user => onConnectedLocalUser = user)
                .AddTo(disposables);

            messagingClient.OnDisconnecting.Subscribe(reason => onDisconnectingReason = reason)
                .AddTo(disposables);

            messagingClient.OnUnexpectedDisconnected.Subscribe(reason => onUnexpectedDisconnectedReason = reason)
                .AddTo(disposables);

            messagingClient.OnUserConnected.Subscribe(user => onUserConnectedRemoteUser = user)
                .AddTo(disposables);

            messagingClient.OnUserDisconnecting.Subscribe(user => onUserDisconnectingUser = user)
                .AddTo(disposables);

            messagingClient.OnMessageReceived.Subscribe(tuple =>
            {
                onMessageReceivedMessageAndUser.user = tuple.userId;
                onMessageReceivedMessageAndUser.message = tuple.message;

            }).AddTo(disposables);

        }
        [TearDown]
        public void DisposeClient()
        {
            disposables.Clear();
        }

        [Test]
        public void SetTransport()
        {
            messagingClient.SetTransport(messagingTransportMock);

            Assert.Pass("No return value to assert, no exception is sufficient.");
        }

        [Test]
        public void SetTransportNull()
            => Assert.That(() => messagingClient.SetTransport(null),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.Contain("transport"));

        [Test]
        public void ConnectFailedTransportNull()
        {
            messagingClient.SetTransport(null);
            var config = new MessagingConnectionConfig("testGroupName1", 2);

            Assert.IsFalse(messagingClient.IsConnected);
            Assert.That(async () => await messagingClient.ConnectAsync(config),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contain("Set Transport before this operation."));
        }

        [Test]
        public void ConnectFailedConfigNull()
        {
            Assert.That(async () => await messagingClient.ConnectAsync(null),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.Contain("connectionConfig"));

            Assert.IsFalse(messagingClient.IsConnected);
        }

        [Test]
        public async void ConnectSuccessAsync()
        {
            var config = new MessagingConnectionConfig("testGroupName1", 2);

            await messagingClient.ConnectAsync(config);

            Assert.IsTrue(messagingClient.IsConnected);
            Assert.AreEqual(expected: onConnectedLocalUser, actual: user1);
            Assert.AreEqual(expected: onUserConnectedRemoteUser, actual: user2);
            Assert.IsTrue(messagingClient.ConnectedUsers.Contains(user1));
        }

        [Test]
        public void SendMessageNull()
        {
            Assert.That(async () => await messagingClient.SendMessageAsync(null),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.Contain("message"));
        }

        [Test]
        public async void SendMessageAsync()
        {
            var message = "test message";
            await messagingClient.SendMessageAsync(message, user2);

            Assert.AreEqual(expected: user2, actual: onMessageReceivedMessageAndUser.user);
            Assert.AreEqual(expected: message, actual: onMessageReceivedMessageAndUser.message);
        }

        [Test]
        public async Task UnexpectedDisconnectAsync()
        {
            var config = new MessagingConnectionConfig("testGroupName2", 2);
            await messagingClient.ConnectAsync(config);

            Assert.IsFalse(messagingClient.IsConnected);
            Assert.AreEqual(expected: "unexpected disconnect", actual: onUnexpectedDisconnectedReason);
        }

        [Test]
        public async void DisconnectAsync()
        {
            await messagingClient.DisconnectAsync();
            Assert.IsFalse(messagingClient.IsConnected);
            Assert.IsFalse(messagingClient.ConnectedUsers.Contains(user1));
            Assert.AreEqual(expected: "disconnecting", actual: onDisconnectingReason);
            Assert.AreEqual(expected: user2, actual: onUserDisconnectingUser);
        }
    }
}
