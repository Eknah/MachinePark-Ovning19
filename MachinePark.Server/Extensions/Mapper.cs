using MachinePark.Server.Entities;
using MachinePark.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinePark.Server.Extensions
{
    public static class Mapper
    {
        public static DeviceTableEntity ToTableEntity(this Device device)
        {
            return new DeviceTableEntity
            {
                Name = device.Name,
                PartitionKey = "Device",
                RowKey = device.Id
            };
        }

        public static Device ToItem(this DeviceTableEntity itemTable)
        {
            return new Device
            {
                Id = itemTable.RowKey,
                Name = itemTable.Name,
                Location = itemTable.Location,
                LastUpdated = itemTable.LastUpdated,
                Type = itemTable.Type,
                IsOnline = itemTable.IsOnline
            };
        }
    }
}
