using System;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    public interface IExtrealMessagingTransport : IDisposable
    {
        bool IsConnected { get; }
        IObservable<string> OnConnected { get; }
        IObservable<Unit> OnDisconnecting { get; }
        IObservable<string> OnConnectionFailed { get; }
        IObservable<string> OnUnexpectedDisconnected { get; }
        IObservable<Unit> OnConnectionApprovalRejected { get; }
        IObservable<string> OnUserConnected { get; }
        IObservable<string> OnUserDisconnecting { get; }
        IObservable<(string userId, string message)> OnMessageReceived { get; }

        UniTask<MessagingRoomInfo[]> ListRoomsAsync();
        UniTask ConnectAsync(MessagingConnectionConfig connectionConfig);
        void Disconnect();
        UniTask DeleteRoomAsync();
        UniTask SendMessageAsync(string message);
    }
}
