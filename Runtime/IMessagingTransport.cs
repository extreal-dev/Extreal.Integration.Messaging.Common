using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    /// <summary>
    /// Interface for implementation communicating with a server.
    /// </summary>
    public interface IMessagingTransport : IDisposable
    {
        /// <summary>
        /// Whether this transport is connected to a group or not.
        /// </summary>
        /// <value>True if connected, false otherwise.</value>
        public bool IsConnected { get; }

        /// <summary>
        /// <para>Invokes immediately after this transport connects to a group.</para>
        /// Arg: User ID of this transport.
        /// </summary>
        public IObservable<string> OnConnected { get; }

        /// <summary>
        /// <para>Invokes just before this transport disconnects from a group.</para>
        /// Arg: reason why this transport disconnects.
        /// </summary>
        public IObservable<string> OnDisconnecting { get; }

        /// <summary>
        /// <para>Invokes immediately after this transport unexpectedly disconnects from the server.</para>
        /// Arg: reason why this transport disconnects.
        /// </summary>
        public IObservable<string> OnUnexpectedDisconnected { get; }

        /// <summary>
        /// Invokes immediately after the connection approval is rejected.
        /// </summary>
        public IObservable<Unit> OnConnectionApprovalRejected { get; }

        /// <summary>
        /// <para>Invokes immediately after a user connects to a group.</para>
        /// Arg: ID of the connected user.
        /// </summary>
        public IObservable<string> OnUserConnected { get; }

        /// <summary>
        /// <para>Invokes just before a user disconnects from a group.</para>
        /// Arg: ID of the disconnected user.
        /// </summary>
        public IObservable<string> OnUserDisconnecting { get; }

        /// <summary>
        /// <para>Invokes immediately after the message is received.</para>
        /// Arg: ID of the user sending the message and the message.
        /// </summary>
        public IObservable<(string userId, string message)> OnMessageReceived { get; }

        /// <summary>
        /// Lists groups that currently exist.
        /// </summary>
        /// <returns>List of the groups that currently exist.</returns>
        UniTask<List<Group>> ListGroupsAsync();

        /// <summary>
        /// Connects to a group.
        /// </summary>
        /// <param name="connectionConfig">Connection Config.</param>
        UniTask ConnectAsync(MessagingConnectionConfig connectionConfig);

        /// <summary>
        /// Disconnects from a group.
        /// </summary>
        UniTask DisconnectAsync();

        /// <summary>
        /// Delete a group that this transport currently connects.
        /// </summary>
        UniTask DeleteGroupAsync();

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <param name="to">
        ///     User ID of the destination.
        ///     <para>Sends a message to the entire group if not specified.</para>
        /// </param>
        UniTask SendMessageAsync(string message, string to = default);
    }
}
