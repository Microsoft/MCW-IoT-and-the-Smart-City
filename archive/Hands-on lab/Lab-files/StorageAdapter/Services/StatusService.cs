// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Models;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services
{
    class StatusService : IStatusService
    {
        private readonly ILogger log;
        private readonly IKeyValueContainer keyValueContainer;
        private readonly IServicesConfig servicesConfig;

        public StatusService(
            ILogger logger,
            IKeyValueContainer keyValueContainer,
            IServicesConfig servicesConfig
            )
        {
            this.log = logger;
            this.keyValueContainer = keyValueContainer;
            this.servicesConfig = servicesConfig;
        }

        public async Task<StatusServiceModel> GetStatusAsync()
        {
            var result = new StatusServiceModel(true, "Alive and well!");
            var errors = new List<string>();

            // Check connection to CosmosDb
            var storageResult = await this.keyValueContainer.PingAsync();
            SetServiceStatus("Storage", storageResult, result, errors);

            result.Properties.Add("StorageType", this.servicesConfig.StorageType);
            this.log.Info(
                "Service status request",
                () => new
                {
                    Healthy = result.Status.IsHealthy,
                    result.Status.Message
                });

            if (errors.Count > 0)
            {
                result.Status.Message = string.Join("; ", errors);
            }
            return result;
        }

        private void SetServiceStatus(
            string dependencyName,
            StatusResultServiceModel serviceResult,
            StatusServiceModel result,
            List<string> errors
            )
        {
            if (!serviceResult.IsHealthy)
            {
                errors.Add(dependencyName + " check failed");
                result.Status.IsHealthy = false;
            }
            result.Dependencies.Add(dependencyName, serviceResult);
        }
    }
}
