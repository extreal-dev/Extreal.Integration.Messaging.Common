using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.Messaging.Common
{
    public class GroupManager : DisposableBase
    {
        private IMessagingTransport transport;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        protected override void ReleaseManagedResources()
            => disposables.Dispose();

        public void SetTransport(IMessagingTransport transport)
        {
            if (transport == null)
            {
                throw new ArgumentNullException(nameof(transport));
            }

            this.transport = transport.AddTo(disposables);
        }

        public UniTask<List<Group>> ListGroupsAsync()
        {
            CheckTransport();
            return transport.ListGroupsAsync();
        }

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
