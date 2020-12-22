// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Models;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services
{
    public interface IStatusService
    {
        Task<StatusServiceModel> GetStatusAsync();
    }
}
