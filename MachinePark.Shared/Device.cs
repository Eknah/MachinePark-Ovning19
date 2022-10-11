using System.ComponentModel;

namespace MachinePark.Shared
{
    public class Device
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public string Name { get; set; } = string.Empty;
        public Location? Location { get; set; } = default;
        public DateTime LastUpdated { get; set; } = default;
        public DeviceType? Type { get; set; } = default;
        public bool IsOnline { get; set; } = false;
    }
}