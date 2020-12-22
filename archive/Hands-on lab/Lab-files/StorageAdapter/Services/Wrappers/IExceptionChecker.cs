// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Wrappers
{
    /// <summary>
    /// Mock support
    /// Since DocumentClientException could not be instanced, unit test must use different exceptions
    /// </summary>
    public interface IExceptionChecker
    {
        bool IsConflictException(Exception exception);
        bool IsPreconditionFailedException(Exception exception);
        bool IsNotFoundException(Exception exception);
    }
}
