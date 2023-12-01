using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    public class MessagingClient : DisposableBase
    {
        public bool IsConnected => transport != null && transport.IsConnected;
        public IReadOnlyList<string> ConnectedUsers => connectedUsers;
        private readonly List<string> connectedUsers = new List<string>();

        public IObservable<string> OnConnected => transport.OnConnected;
        public IObservable<Unit> OnDisconnecting => transport.OnDisconnecting;
        public IObservable<string> OnConnectionFailed => transport.OnConnectionFailed;
        public IObservable<string> OnUnexpectedDisconnected => transport.OnUnexpectedDisconnected;
        public IObservable<Unit> OnConnectionApprovalRejected => transport.OnConnectionApprovalRejected;
        public IObservable<string> OnUserConnected => transport.OnUserConnected;
        public IObservable<string> OnUserDisconnecting => transport.OnUserDisconnecting;
        public IObservable<(string userId, string message)> OnMessageReceived => transport.OnMessageReceived;

        private IExtrealMessagingTransport transport;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        protected override void ReleaseManagedResources()
            => disposables.Dispose();

        public void SetTransport(IExtrealMessagingTransport transport)
        {
            this.transport = transport.AddTo(disposables);

            this.transport.OnUserConnected
                .Subscribe(connectedUsers.Add)
                .AddTo(disposables);

            this.transport.OnUserDisconnecting
                .Subscribe(userId => connectedUsers.Remove(userId))
                .AddTo(disposables);
        }

        public UniTask<MessagingRoomInfo[]> ListRoomsAsync()
            => transport.ListRoomsAsync();

        public UniTask ConnectAsync(MessagingConnectionConfig connectionConfig)
            => transport.ConnectAsync(connectionConfig);

        public void Disconnect()
            => transport.Disconnect();

        public UniTask DeleteRoomAsync()
            => transport.DeleteRoomAsync();

        public UniTask SendMessageAsync(string message)
            => transport.SendMessageAsync(message);
    }
}
