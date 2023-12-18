using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using System;
using System.Collections.Generic;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    /// <summary>
    /// Class for group messaging.
    /// </summary>
    public class MessagingClient : DisposableBase
    {
        /// <summary>
        /// Whether this client is connected to a group or not.
        /// </summary>
        /// <value>True if connected, false otherwise.</value>
        public bool IsConnected => transport != null && transport.IsConnected;

        /// <summary>
        /// IDs of connected users.
        /// </summary>
        public IReadOnlyList<string> ConnectedUsers => connectedUsers;
        private readonly List<string> connectedUsers = new List<string>();

        /// <summary>
        /// <para>Invokes immediately after this client connects to a group.</para>
        /// Arg: User ID of this client.
        /// </summary>
        public IObservable<string> OnConnected => transport.OnConnected;

        /// <summary>
        /// <para>Invokes just before this client disconnects from a group.</para>
        /// Arg: reason why this client disconnects.
        /// </summary>
        public IObservable<string> OnDisconnecting => transport.OnDisconnecting;

        /// <summary>
        /// <para>Invokes immediately after this client unexpectedly disconnects from the server.</para>
        /// Arg: reason why this client disconnects.
        /// </summary>
        public IObservable<string> OnUnexpectedDisconnected => transport.OnUnexpectedDisconnected;

        /// <summary>
        /// Invokes immediately after the connection approval is rejected.
        /// </summary>
        public IObservable<Unit> OnConnectionApprovalRejected => transport.OnConnectionApprovalRejected;

        /// <summary>
        /// <para>Invokes immediately after a user connects to a group.</para>
        /// Arg: ID of the connected user.
        /// </summary>
        public IObservable<string> OnUserConnected => transport.OnUserConnected;

        /// <summary>
        /// <para>Invokes just before a user disconnects from a group.</para>
        /// Arg: ID of the disconnected user.
        /// </summary>
        public IObservable<string> OnUserDisconnecting => transport.OnUserDisconnecting;

        /// <summary>
        /// <para>Invokes immediately after the message is received.</para>
        /// Arg: ID of the user sending the message and the message.
        /// </summary>
        public IObservable<(string userId, string message)> OnMessageReceived => transport.OnMessageReceived;

        private IMessagingTransport transport;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        protected override void ReleaseManagedResources()
            => disposables.Dispose();

        /// <summary>
        /// Sets a transport.
        /// </summary>
        /// <param name="transport">Transport that implements IMessagingTransport.</param>
        /// <exception cref="ArgumentNullException">When transport is null.</exception>
        public void SetTransport(IMessagingTransport transport)
        {
            if (transport == null)
            {
                throw new ArgumentNullException(nameof(transport));
            }

            this.transport = transport.AddTo(disposables);

            this.transport.OnUserConnected
                .Subscribe(connectedUsers.Add)
                .AddTo(disposables);

            this.transport.OnUserDisconnecting
                .Subscribe(userId => connectedUsers.Remove(userId))
                .AddTo(disposables);
        }

        /// <summary>
        /// Connects to a group.
        /// </summary>
        /// <param name="connectionConfig">Connection Config.</param>
        public UniTask ConnectAsync(MessagingConnectionConfig connectionConfig)
        {
            CheckTransport();

            if (connectionConfig == null)
            {
                throw new ArgumentNullException(nameof(connectionConfig));
            }

            return transport.ConnectAsync(connectionConfig);
        }

        /// <summary>
        /// Disconnects from a group.
        /// </summary>
        public UniTask DisconnectAsync()
        {
            CheckTransport();
            return transport.DisconnectAsync();
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <param name="to">
        ///     User ID of the destination.
        ///     <para>Sends a message to the entire group if not specified.</para>
        /// </param>
        public UniTask SendMessageAsync(string message, string to = default)
        {
            CheckTransport();

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            return transport.SendMessageAsync(message, to);
        }

        private void CheckTransport()
        {
            if (transport == null)
            {
                throw new InvalidOperationException("Set Transport before this operation.");
            }
        }
    }
}
