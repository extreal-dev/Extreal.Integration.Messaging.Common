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
            => this.transport = transport.AddTo(disposables);

        public UniTask<List<Group>> ListGroupsAsync()
            => transport.ListGroupsAsync();

        public UniTask DeleteGroupAsync()
            => transport.DeleteGroupAsync();
    }
}
