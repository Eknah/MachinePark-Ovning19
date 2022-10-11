using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using MachinePark.Shared;
using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations.Schema;
using TableAttribute = Microsoft.Azure.WebJobs.TableAttribute;

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using CloudTable = Microsoft.Azure.Cosmos.Table.CloudTable;
using MachinePark.Server.Entities;
using MachinePark.Server.Extensions;
using System.Linq;
using TableOperation = Microsoft.Azure.Cosmos.Table.TableOperation;

namespace MachinePark.Server
{
    public static class MachineParkAPI
    {

        [FunctionName("GetDevices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Table("devices", Connection = "AzureWebJobsStorage")] CloudTable deviceTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var query = new Microsoft.Azure.Cosmos.Table.TableQuery<DeviceTableEntity>();
            var result = await deviceTable.ExecuteQuerySegmentedAsync(query, null);

            var response = result.Select(Mapper.ToItem).ToList();

            return new OkObjectResult(response);
        }


        [FunctionName("CreateDevice")]
        public static async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("devices", Connection = "AzureWebJobsStorage")] CloudTable deviceTable, // IAsyncCollector<ItemTableEntity> itemTable,
            ILogger log)
        {
            log.LogInformation("Create item");

            Location defaultLocation = new() { Name = "Vega", Country = "Sweden" };
            DeviceType defaultType = new() { Name = "Weather Sensor", Description = "temperature, humidity" };

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var createDevice = JsonConvert.DeserializeObject<CreateDevice>(requestBody);

            if (createDevice == null || string.IsNullOrWhiteSpace(createDevice.Name)) return new BadRequestResult();

            var device = new Device
            {
                Name = createDevice.Name,
                Location = defaultLocation,
                Type = defaultType,
                LastUpdated = DateTime.Now.AddDays(Random.Shared.Next(20)).Date,
                IsOnline = false
            };

            // await itemTable.AddAsync(item.ToTableEntity());

            var operation = TableOperation.Insert(device.ToTableEntity());
            var res = await deviceTable.ExecuteAsync(operation);

            return new OkObjectResult(device);
        }



        [FunctionName("DeleteDevice")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteDevice/{id}")] HttpRequest req,
            [Table("devices", "Device", "{id}", Connection = "AzureWebJobsStorage")] DeviceTableEntity deviceTableToDelete,
            [Table("devices", Connection = "AzureWebJobsStorage")] CloudTable deviceTable,
            ILogger log, string id)
        {
            log.LogInformation("Delete item");


            if (deviceTableToDelete == null || string.IsNullOrWhiteSpace(deviceTableToDelete.Name)) return new BadRequestResult();

            var operation = TableOperation.Delete(deviceTableToDelete);
            await deviceTable.ExecuteAsync(operation);
            return new NoContentResult();
        }


        [FunctionName("EditDevice")]
        public static async Task<IActionResult> Put(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "EditDevice/{id}")] HttpRequest req,
            [Table("devices", Connection = "AzureWebJobsStorage")] CloudTable deviceTable,
            ILogger log, string id)
        {
            log.LogInformation("Put item");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var deviceToUpdate = JsonConvert.DeserializeObject<EditDevice>(requestBody);

            if (deviceToUpdate is null || string.IsNullOrEmpty(id)) return new BadRequestResult();

            var opertaion = TableOperation.Retrieve<DeviceTableEntity>("Device", id);
            var found = await deviceTable.ExecuteAsync(opertaion);

            if (found.Result == null) return new NotFoundResult();

            var existingDevice = found.Result as DeviceTableEntity;
            existingDevice.IsOnline = deviceToUpdate.IsOnline;

            var opertionReplace = TableOperation.Replace(existingDevice);
            await deviceTable.ExecuteAsync(opertionReplace);
            //ToDo check if ok

            return new NoContentResult();
        }


    }
}
