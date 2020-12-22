// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services;
using Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.Runtime;
using Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.v1.Models;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.v1.Controllers
{
    [Route(Version.PATH + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public sealed class StatusController : Controller
    {
        private readonly IConfig config;
        private readonly IStatusService statusService;

        public StatusController(IConfig config, IStatusService statusService)
        {
            this.config = config;
            this.statusService = statusService;
        }

        public async Task<StatusApiModel> GetAsync()
        {
            var result = new StatusApiModel(await this.statusService.GetStatusAsync());

            result.Properties.Add("Port", this.config.Port.ToString());
            return result;
        }
    }
}
