namespace Extreal.Integration.Messaging.Common
{
    public class MessagingRoomInfo
    {
        public string Id { get; }
        public string Name { get; }

        public MessagingRoomInfo(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
