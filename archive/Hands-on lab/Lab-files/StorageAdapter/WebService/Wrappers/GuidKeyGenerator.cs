// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.Wrappers
{
    public class GuidKeyGenerator : IKeyGenerator
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
