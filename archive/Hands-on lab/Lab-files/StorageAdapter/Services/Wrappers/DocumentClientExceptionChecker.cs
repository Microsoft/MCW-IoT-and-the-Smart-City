// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net;
using Microsoft.Azure.Documents;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Wrappers
{
    public class DocumentClientExceptionChecker : IExceptionChecker
    {
        public bool IsConflictException(Exception exception)
        {
            var ex = exception as DocumentClientException;
            return ex != null && ex.StatusCode == HttpStatusCode.Conflict;
        }

        public bool IsPreconditionFailedException(Exception exception)
        {
            var ex = exception as DocumentClientException;
            return ex != null && ex.StatusCode == HttpStatusCode.PreconditionFailed;
        }

        public bool IsNotFoundException(Exception exception)
        {
            var ex = exception as DocumentClientException;
            return ex != null && ex.StatusCode == HttpStatusCode.NotFound;
        }
    }
}
