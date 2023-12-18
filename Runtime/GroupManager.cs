using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    /// <summary>
    /// Class that manages the groups.
    /// </summary>
    public class GroupManager : DisposableBase
    {
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
        }

        /// <summary>
        /// Lists groups that currently exist.
        /// </summary>
        /// <exception cref="InvalidOperationException">When call this method before setting a transport.</exception>
        /// <returns>List of the groups that currently exist.</returns>
        public UniTask<List<Group>> ListGroupsAsync()
        {
            CheckTransport();
            return transport.ListGroupsAsync();
        }

        /// <summary>
        /// Delete a group that this transport currently connects.
        /// </summary>
        /// <exception cref="InvalidOperationException">When call this method before setting a transport.</exception>
        public UniTask DeleteGroupAsync()
        {
            CheckTransport();
            return transport.DeleteGroupAsync();
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
