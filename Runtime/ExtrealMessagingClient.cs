using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using System;
using System.Collections.Generic;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    public class ExtrealMessagingClient : DisposableBase
    {
        public bool IsConnected => transport != null && transport.IsConnected;

        public IReadOnlyList<string> ConnectedUsers => connectedUsers;
        private readonly List<string> connectedUsers = new List<string>();

        public IObservable<string> OnConnected => transport.OnConnected;
        public IObservable<string> OnDisconnecting => transport.OnDisconnecting;
        public IObservable<string> OnUnexpectedDisconnected => transport.OnUnexpectedDisconnected;
        public IObservable<Unit> OnConnectionApprovalRejected => transport.OnConnectionApprovalRejected;
        public IObservable<string> OnUserConnected => transport.OnUserConnected;
        public IObservable<string> OnUserDisconnecting => transport.OnUserDisconnecting;
        public IObservable<(string userId, string message)> OnMessageReceived => transport.OnMessageReceived;

        private IExtrealMessagingTransport transport;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        protected override void ReleaseManagedResources()
            => disposables.Dispose();

        public void SetTransport(IExtrealMessagingTransport messagingTransport)
        {
            transport = messagingTransport.AddTo(disposables);

            transport.OnUserConnected
                .Subscribe(connectedUsers.Add)
                .AddTo(disposables);

            transport.OnUserDisconnecting
                .Subscribe(userId => connectedUsers.Remove(userId))
                .AddTo(disposables);
        }

        public UniTask<List<MessagingRoomInfo>> ListRoomsAsync()
            => transport.ListRoomsAsync();

        public UniTask ConnectAsync(MessagingConnectionConfig connectionConfig)
            => transport.ConnectAsync(connectionConfig);

        public UniTask DisconnectAsync()
            => transport.DisconnectAsync();

        public UniTask DeleteRoomAsync()
            => transport.DeleteRoomAsync();

        public UniTask SendMessageAsync(string jsonMsg, string to = default)
            => transport.SendMessageAsync(jsonMsg, to);
    }
}
