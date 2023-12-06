namespace Extreal.Integration.Messaging.Common
{
    public class MessagingConnectionConfig
    {
        public string RoomName { get; }
        public int MaxCapacity { get; }

        public MessagingConnectionConfig(string roomName, int maxCapacity = default)
        {
            RoomName = roomName;
            MaxCapacity = maxCapacity;
        }
    }
}
