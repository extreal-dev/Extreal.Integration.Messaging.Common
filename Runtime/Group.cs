namespace Extreal.Integration.Messaging.Common
{
    public class Group
    {
        public string Id { get; }
        public string Name { get; }

        public Group(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
