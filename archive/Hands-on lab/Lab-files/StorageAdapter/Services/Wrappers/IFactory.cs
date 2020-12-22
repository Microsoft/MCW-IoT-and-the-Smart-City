// Copyright (c) Microsoft. All rights reserved.


namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Wrappers
{
    /// <summary>
    /// Mock support
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<out T>
    {
        T Create();
    }
}
