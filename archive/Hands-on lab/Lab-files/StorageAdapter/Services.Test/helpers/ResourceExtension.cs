// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.Azure.Documents;

namespace Services.Test.helpers
{
    internal static class ResourceExtension
    {
        public static void SetETag(this Resource resource, string etag)
        {
            resource.SetPropertyValue("_etag", etag);
        }

        public static void SetTimestamp(this Resource resource, DateTimeOffset timestamp)
        {
            resource.SetPropertyValue("_ts", timestamp.ToUnixTimeSeconds());
        }
    }
}
