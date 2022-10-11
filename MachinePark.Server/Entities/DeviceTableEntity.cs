using MachinePark.Shared;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachinePark.Server.Entities
{
    public class DeviceTableEntity : TableEntity
    {
        public string Name { get; set; } = string.Empty;
        public Location? Location { get; set; } = new() { Name = "Vega", Country = "Sweden" };
        public DateTime LastUpdated { get; set; } = DateTime.Now.AddDays(Random.Shared.Next(20)).Date;
        public DeviceType? Type { get; set; } = new() { Name = "Weather Sensor", Description = "temperature, humidity" };
        public bool IsOnline { get; set; } = false;
    }
}
