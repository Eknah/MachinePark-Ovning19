using MachinePark.Shared;
using System.Net.Http.Json;

namespace MachinePark.Services
{
    public class DevicesClient : IDevicesClient
    {
        private readonly HttpClient httpClient;

        public DevicesClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Device>> GetAsync()
        {
            var result = await httpClient.GetFromJsonAsync<IEnumerable<Device>>("api/GetDevices");
            return result;
        }

        public async Task<Device?> PostAsync(CreateDevice createDevice)
        {
            var response = await httpClient.PostAsJsonAsync<CreateDevice>("api/CreateDevice", createDevice);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Device>();

            return null;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var response = await httpClient.DeleteAsync($"api/DeleteDevice/{id}");

            return response.IsSuccessStatusCode ? true : false;
        }

        public async Task<bool> EditAsync(Device device)
        {
            //Borde skickats in här istället...
            var updateDevice = new EditDevice { IsOnline= device.IsOnline};

            var response = await httpClient.PutAsJsonAsync($"api/EditDevice/{device.Id}", updateDevice);

            return response.IsSuccessStatusCode ? true : false;
        }
    }
}
