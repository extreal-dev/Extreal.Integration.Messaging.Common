namespace Extreal.Integration.Messaging.Common
{
    public class GroupConfig
    {
        public string GroupName { get; }
        public int MaxCapacity { get; }

        public GroupConfig(string groupName, int maxCapacity)
        {
            GroupName = groupName;
            MaxCapacity = maxCapacity;
        }
    }
}
