namespace Extreal.Integration.Messaging.Common
{
    public class MessagingConnectionConfig
    {
        public string RoomName { get; }

        public MessagingConnectionConfig(string roomName)
            => RoomName = roomName;
    }
}
