using System;
using UnityEngine;

namespace Extreal.Integration.Messaging.Common
{
    [Serializable]
    public class MessagingRoomInfo
    {
        public string Id => id;
        [SerializeField] private string id;

        public string Name => name;
        [SerializeField] private string name;
    }
}
