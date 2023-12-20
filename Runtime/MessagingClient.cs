using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    /// <summary>
    /// Class for group messaging.
    /// </summary>
    public abstract class MessagingClient : DisposableBase
    {
        /// <summary>
        /// Whether this client is connected to a group or not.
        /// </summary>
        /// <value>True if connected, false otherwise.</value>
        public bool IsJoinedGroup { get; private set; }
        protected void SetJoiningGroupStatus(bool isJoinedGroup)
            => IsJoinedGroup = isJoinedGroup;

        /// <summary>
        /// IDs of connected users.
        /// </summary>
        public IReadOnlyList<string> JoinedUsers => joinedUsers;
        private readonly List<string> joinedUsers = new List<string>();

        /// <summary>
        /// <para>Invokes immediately after this client connects to a group.</para>
        /// Arg: User ID of this client.
        /// </summary>
        public IObservable<string> OnJoined => onJoined;
        private readonly Subject<string> onJoined;
        protected void FireOnJoined(string userId) => UniTask.Void(async () =>
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnJoined)}: userId={userId}");
            }
            onJoined.OnNext(userId);
        });

        /// <summary>
        /// <para>Invokes just before this client disconnects from a group.</para>
        /// Arg: reason why this client disconnects.
        /// </summary>
        public IObservable<string> OnLeaving => onLeaving;
        private readonly Subject<string> onLeaving;
        protected void FireOnLeaving(string reason) => UniTask.Void(async () =>
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnLeaving)}: reason={reason}");
            }
            onLeaving.OnNext(reason);
        });

        /// <summary>
        /// <para>Invokes immediately after this client unexpectedly disconnects from the server.</para>
        /// Arg: reason why this client disconnects.
        /// </summary>
        public IObservable<string> OnUnexpectedLeft => onUnexpectedLeft;
        private readonly Subject<string> onUnexpectedLeft;
        protected void FireOnUnexpectedLeft(string reason) => UniTask.Void(async () =>
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnUnexpectedLeft)}: reason={reason}");
            }
            SetJoiningGroupStatus(false);
            onUnexpectedLeft.OnNext(reason);
        });

        /// <summary>
        /// Invokes immediately after the connection approval is rejected.
        /// </summary>
        public IObservable<Unit> OnJoiningApprovalRejected => onJoiningApprovalRejected;
        private readonly Subject<Unit> onJoiningApprovalRejected;
        protected void FireOnJoiningApprovalRejected() => UniTask.Void(async () =>
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnJoiningApprovalRejected)}");
            }
            onJoiningApprovalRejected.OnNext(Unit.Default);
        });

        /// <summary>
        /// <para>Invokes immediately after a user connects to a group.</para>
        /// Arg: ID of the connected user.
        /// </summary>
        public IObservable<string> OnUserJoined => onUserJoined;
        private readonly Subject<string> onUserJoined;
        protected void FireOnUserJoined(string userId) => UniTask.Void(async () =>
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnUserJoined)}: userId={userId}");
            }
            onUserJoined.OnNext(userId);
        });

        /// <summary>
        /// <para>Invokes just before a user disconnects from a group.</para>
        /// Arg: ID of the disconnected user.
        /// </summary>
        public IObservable<string> OnUserLeaving => onUserLeaving;
        private readonly Subject<string> onUserLeaving;
        protected void FireOnUserLeaving(string userId) => UniTask.Void(async () =>
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnUserLeaving)}: userId={userId}");
            }
            onUserLeaving.OnNext(userId);
        });

        /// <summary>
        /// <para>Invokes immediately after the message is received.</para>
        /// Arg: ID of the user sending the message and the message.
        /// </summary>
        public IObservable<(string userId, string message)> OnMessageReceived => onMessageReceived;
        private readonly Subject<(string, string)> onMessageReceived;
        protected void FireOnMessageReceived(string userId, string message) => UniTask.Void(async () =>
        {
            await UniTask.SwitchToMainThread();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnMessageReceived)}: userId={userId}, message={message}");
            }
            onMessageReceived.OnNext((userId, message));
        });

        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(MessagingClient));

        /// <summary>
        /// Creates a new MessagingClient.
        /// </summary>
        [SuppressMessage("Usage", "CC0022")]
        protected MessagingClient()
        {
            onJoined = new Subject<string>().AddTo(disposables);
            onLeaving = new Subject<string>().AddTo(disposables);
            onUnexpectedLeft = new Subject<string>().AddTo(disposables);
            onUserJoined = new Subject<string>().AddTo(disposables);
            onUserLeaving = new Subject<string>().AddTo(disposables);
            onJoiningApprovalRejected = new Subject<Unit>().AddTo(disposables);
            onMessageReceived = new Subject<(string, string)>().AddTo(disposables);

            OnUserJoined
                .Subscribe(joinedUsers.Add)
                .AddTo(disposables);

            OnUserLeaving
                .Subscribe(userId => joinedUsers.Remove(userId))
                .AddTo(disposables);
        }

        protected sealed override void ReleaseManagedResources()
        {
            disposables.Dispose();
            DoReleaseManagedResources();
        }

        protected virtual void DoReleaseManagedResources() { }

        /// <summary>
        /// Lists groups that currently exist.
        /// </summary>
        /// <returns>List of the groups that currently exist.</returns>
        public async UniTask<List<Group>> ListGroupsAsync()
        {
            var groupList = await DoListGroupsAsync();
            return groupList.Groups.Select(groupResponse => new Group(groupResponse.Id, groupResponse.Name)).ToList();
        }

        protected abstract UniTask<GroupListResponse> DoListGroupsAsync();

        public async UniTask CreateGroupAsync(GroupConfig groupConfig)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Create group: GroupName={groupConfig.GroupName}, MaxCapacity={groupConfig.MaxCapacity}");
            }

            var createGroupResponse = await DoCreateGroupAsync(groupConfig);
            if (createGroupResponse.Status == 409)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(createGroupResponse.Message);
                }
                throw new GroupNameAlreadyExistsException(createGroupResponse.Message);
            }
        }

        protected abstract UniTask<CreateGroupResponse> DoCreateGroupAsync(GroupConfig groupConfig);

        /// <summary>
        /// Delete a group that this transport currently connects.
        /// </summary>
        /// <param name="groupName">Group name to be deleted.</param>
        public abstract UniTask DeleteGroupAsync(string groupName);

        /// <summary>
        /// Connects to a group.
        /// </summary>
        /// <param name="connectionConfig">Connection Config.</param>
        public UniTask JoinAsync(MessagingJoiningConfig connectionConfig)
        {
            if (connectionConfig == null)
            {
                throw new ArgumentNullException(nameof(connectionConfig));
            }

            return DoJoinAsync(connectionConfig);
        }

        protected abstract UniTask DoJoinAsync(MessagingJoiningConfig connectionConfig);

        /// <summary>
        /// Disconnects from a group.
        /// </summary>
        public UniTask LeaveAsync()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(nameof(LeaveAsync));
            }
            FireOnLeaving("disconnect request");
            return DoLeaveAsync();
        }

        protected abstract UniTask DoLeaveAsync();

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <param name="to">
        ///     User ID of the destination.
        ///     <para>Sends a message to the entire group if not specified.</para>
        /// </param>
        public async UniTask SendMessageAsync(string message, string to = default)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!IsJoinedGroup)
            {
                if (Logger.IsWarn())
                {
                    Logger.LogWarn("Called Send method before connecting to a group");
                }
                return;
            }

            await DoSendMessageAsync(message, to);
        }

        protected abstract UniTask DoSendMessageAsync(string message, string to);
    }
}
