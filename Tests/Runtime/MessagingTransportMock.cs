using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Extreal.Integration.Messaging.Common.Test
{
    public class MessagingTransportMock : IMessagingTransport
    {
        public bool IsConnected { get; private set; }

        public IObservable<string> OnConnected => onConnected;
        private readonly Subject<string> onConnected;

        public IObservable<string> OnDisconnecting => onDisconnecting;
        private readonly Subject<string> onDisconnecting;

        public IObservable<string> OnUnexpectedDisconnected => onUnexpectedDisconnected;
        private readonly Subject<string> onUnexpectedDisconnected;

        public IObservable<Unit> OnConnectionApprovalRejected => onConnectionApprovalRejected;
        private readonly Subject<Unit> onConnectionApprovalRejected;

        public IObservable<string> OnUserConnected => onUserConnected;
        private readonly Subject<string> onUserConnected;

        public IObservable<string> OnUserDisconnecting => onUserDisconnecting;
        private readonly Subject<string> onUserDisconnecting;

        public IObservable<(string userId, string message)> OnMessageReceived => onMessageReceived;
        private readonly Subject<(string, string)> onMessageReceived;

        private readonly string user1 = "testUser1";
        private readonly string user2 = "testUser2";

        public UniTask ConnectAsync(MessagingConnectionConfig connectionConfig)
        {
            if (connectionConfig.GroupName != "testGroupName1")
            {
                IsConnected = false;
                onUnexpectedDisconnected.OnNext("unexpected disconnect");
                onConnectionApprovalRejected.OnNext(Unit.Default);
            }

            IsConnected = true;
            onConnected.OnNext(user1);
            onUserConnected.OnNext(user2);

            return UniTask.CompletedTask;
        }

        public UniTask DeleteGroupAsync() => UniTask.CompletedTask;

        public UniTask DisconnectAsync()
        {
            IsConnected = false;
            onDisconnecting.OnNext("disconnecting");
            onUserDisconnecting.OnNext(user2);
            return UniTask.CompletedTask;
        }

        public void Dispose() => throw new NotImplementedException();

        public UniTask<List<Group>> ListGroupsAsync()
        {
            var groups = new List<Group> { new("testId1", "testGroupName1"), new("testId2", "testGroupName2") };

            return UniTask.FromResult(groups);
        }

        public UniTask SendMessageAsync(string message, string to = null)
        {
            onMessageReceived.OnNext((message, to));
            return UniTask.CompletedTask;
        }

        public void DisposeMock()
        {
            // 必要なクリーンアップ処理を記述
            onConnected.Dispose();
            onDisconnecting.Dispose();
            onUnexpectedDisconnected.Dispose();
            onConnectionApprovalRejected.Dispose();
            onUserConnected.Dispose();
            onUserDisconnecting.Dispose();
            onMessageReceived.Dispose();

            // その他のリソース解放処理
        }
    }
}
