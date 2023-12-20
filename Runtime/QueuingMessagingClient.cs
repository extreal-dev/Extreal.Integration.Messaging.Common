using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using System;
using System.Collections.Generic;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    /// <summary>
    /// Class that wraps MessagingClient so that queuing can be used.
    /// </summary>
    public class QueuingMessagingClient : DisposableBase
    {
        /// <summary>
        /// Whether this client is connected to a group or not.
        /// </summary>
        /// <value>True if connected, false otherwise.</value>
        public bool IsJoinedGroup => messagingClient.IsJoinedGroup;

        /// <summary>
        /// IDs of connected users.
        /// </summary>
        public IReadOnlyList<string> JoinedUsers => messagingClient.JoinedUsers;

        /// <summary>
        /// <para>Invokes immediately after this client connects to a group.</para>
        /// Arg: User ID of this client.
        /// </summary>
        public IObservable<string> OnJoined => messagingClient.OnJoined;

        /// <summary>
        /// <para>Invokes just before this client disconnects from a group.</para>
        /// Arg: reason why this client disconnects.
        /// </summary>
        public IObservable<string> OnLeaving => messagingClient.OnLeaving;

        /// <summary>
        /// <para>Invokes immediately after this client unexpectedly disconnects from the server.</para>
        /// Arg: reason why this client disconnects.
        /// </summary>
        public IObservable<string> OnUnexpectedLeft => messagingClient.OnUnexpectedLeft;

        /// <summary>
        /// Invokes immediately after the connection approval is rejected.
        /// </summary>
        public IObservable<Unit> OnJoiningApprovalRejected => messagingClient.OnJoiningApprovalRejected;

        /// <summary>
        /// <para>Invokes immediately after a user connects to a group.</para>
        /// Arg: ID of the connected user.
        /// </summary>
        public IObservable<string> OnUserJoined => messagingClient.OnUserJoined;

        /// <summary>
        /// <para>Invokes just before a user disconnects from a group.</para>
        /// Arg: ID of the disconnected user.
        /// </summary>
        public IObservable<string> OnUserLeaving => messagingClient.OnUserLeaving;

        private readonly MessagingClient messagingClient;

        private readonly Queue<(string, string)> requestQueue = new Queue<(string, string)>();
        private readonly Queue<(string, string)> responseQueue = new Queue<(string, string)>();

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        /// <summary>
        /// Creates a new QueuingMessagingClient.
        /// </summary>
        /// <param name="messagingClient">MessagingClient.</param>
        /// <exception cref="ArgumentNullException">When messagingClient is null.</exception>
        public QueuingMessagingClient(MessagingClient messagingClient)
        {
            if (messagingClient == null)
            {
                throw new ArgumentNullException(nameof(messagingClient));
            }

            this.messagingClient = messagingClient.AddTo(disposables);

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
                if (IsJoinedGroup)
                {
                    await messagingClient.SendMessageAsync(message, to);
                }
            }
        }

        /// <summary>
        /// Enqueues message to request queue.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <param name="to">
        ///     User ID of the destination.
        ///     <para>Sends a message to the entire group if not specified.</para>
        /// </param>
        public void EnqueueRequest(string message, string to = default)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            requestQueue.Enqueue((to, message));
        }

        /// <summary>
        /// Counts the number of elements in the response queue.
        /// </summary>
        /// <returns>Number of elements in the response queue.</returns>
        public int ResponseQueueCount()
            => responseQueue.Count;

        /// <summary>
        /// Dequeues from response queue.
        /// </summary>
        /// <returns>ID of the user sending the message and the message.</returns>
        public (string from, string message) DequeueResponse()
            => responseQueue.Dequeue();

        /// <summary>
        /// Lists groups that currently exist.
        /// </summary>
        /// <returns>List of the groups that currently exist.</returns>
        public UniTask<List<Group>> ListGroupsAsync()
            => messagingClient.ListGroupsAsync();

        public UniTask CreateGroupAsync(GroupConfig groupConfig)
            => messagingClient.CreateGroupAsync(groupConfig);

        /// <summary>
        /// Delete a group that this transport currently connects.
        /// </summary>
        /// <param name="groupName">Group name to be deleted.</param>
        public UniTask DeleteGroupAsync(string groupName)
            => messagingClient.DeleteGroupAsync(groupName);

        /// <summary>
        /// Connects to a group.
        /// </summary>
        /// <param name="connectionConfig">Connection Config.</param>
        public UniTask JoinAsync(MessagingJoiningConfig connectionConfig)
            => messagingClient.JoinAsync(connectionConfig);

        /// <summary>
        /// Disconnects from a group.
        /// </summary>
        public UniTask LeaveAsync()
            => messagingClient.LeaveAsync();
    }
}
