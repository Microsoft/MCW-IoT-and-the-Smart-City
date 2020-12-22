// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Wrappers;

namespace Services.Test.helpers
{
    public class MockExceptionChecker : IExceptionChecker
    {
        public bool IsConflictException(Exception exception)
        {
            return exception is ConflictingResourceException;
        }

        public bool IsPreconditionFailedException(Exception exception)
        {
            return exception is ConflictingResourceException;
        }

        public bool IsNotFoundException(Exception exception)
        {
            return exception is ResourceNotFoundException;
        }
    }
}
