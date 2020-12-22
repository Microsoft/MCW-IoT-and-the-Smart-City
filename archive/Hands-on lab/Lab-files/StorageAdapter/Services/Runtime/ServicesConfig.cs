// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime
{
    public interface IServicesConfig
    {
        string StorageType { get; set; }
        string DocumentDbConnString { get; set; }
        string DocumentDbDatabase { get; set; }
        string DocumentDbCollection { get; set; }
        int DocumentDbRUs { get; set; }
    }

    public class ServicesConfig : IServicesConfig
    {
        public string StorageType { get; set; }
        public string DocumentDbConnString { get; set; }
        public string DocumentDbDatabase { get; set; }
        public string DocumentDbCollection { get; set; }
        public int DocumentDbRUs { get; set; }
    }
}
