using MachinePark.Shared;

namespace MachinePark.Services
{
    public interface IDevicesClient
    {
        Task<IEnumerable<Device>> GetAsync();
        Task<Device?> PostAsync(CreateDevice createDevice);
        Task<bool> RemoveAsync(string id);
        Task<bool> EditAsync(Device item);
    }
}
