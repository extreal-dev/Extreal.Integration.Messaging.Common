using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using System;
using System.Collections.Generic;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    public class QueuingMessagingClient : DisposableBase
    {
        public bool IsConnected => messagingClient.IsConnected;

        public IReadOnlyList<string> ConnectedUsers => messagingClient.ConnectedUsers;

        public IObservable<string> OnConnected => messagingClient.OnConnected;
        public IObservable<string> OnDisconnecting => messagingClient.OnDisconnecting;
        public IObservable<string> OnUnexpectedDisconnected => messagingClient.OnUnexpectedDisconnected;
        public IObservable<Unit> OnConnectionApprovalRejected => messagingClient.OnConnectionApprovalRejected;
        public IObservable<string> OnUserConnected => messagingClient.OnUserConnected;
        public IObservable<string> OnUserDisconnecting => messagingClient.OnUserDisconnecting;

        private readonly MessagingClient messagingClient;

        private readonly Queue<(string, string)> requestQueue = new Queue<(string, string)>();
        private readonly Queue<(string, string)> responseQueue = new Queue<(string, string)>();

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public QueuingMessagingClient(MessagingClient messagingClient)
        {
            this.messagingClient = messagingClient;

            messagingClient.OnMessageReceived
                .Subscribe(responseQueue.Enqueue)
                .AddTo(disposables);

            Observable.EveryUpdate()
                .Subscribe(_ => UpdateAsync().Forget())
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources()
            => disposables.Dispose();

        private async UniTaskVoid UpdateAsync()
        {
            while (requestQueue.Count > 0)
            {
                (var to, var message) = requestQueue.Dequeue();
                if (IsConnected)
                {
                    await messagingClient.SendMessageAsync(message, to);
                }
            }
        }

        public void EnqueueRequest(string message, string to = default)
            => requestQueue.Enqueue((to, message));

        public int ResponseQueueCount()
            => responseQueue.Count;

        public (string from, string message) DequeueResponse()
            => responseQueue.Dequeue();

        public UniTask ConnectAsync(MessagingConnectionConfig connectionConfig)
            => messagingClient.ConnectAsync(connectionConfig);

        public UniTask DisconnectAsync()
            => messagingClient.DisconnectAsync();
    }
}
