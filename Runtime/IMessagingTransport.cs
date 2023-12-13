using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    public interface IMessagingTransport : IDisposable
    {
        public bool IsConnected { get; }
        public IObservable<string> OnConnected { get; }
        public IObservable<string> OnDisconnecting { get; }
        public IObservable<string> OnUnexpectedDisconnected { get; }
        public IObservable<Unit> OnConnectionApprovalRejected { get; }
        public IObservable<string> OnUserConnected { get; }
        public IObservable<string> OnUserDisconnecting { get; }
        public IObservable<(string userId, string message)> OnMessageReceived { get; }

        UniTask SendMessageAsync(string message, string to = default);
        UniTask<List<Group>> ListGroupsAsync();
        UniTask ConnectAsync(MessagingConnectionConfig connectionConfig);
        UniTask DisconnectAsync();
        UniTask DeleteGroupAsync();
    }
}
