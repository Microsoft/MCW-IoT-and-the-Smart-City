// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Helpers
{
    public static class DocumentIdHelper
    {
        /// <summary>
        /// To reduce cost, we will use single document collection. So the actual document
        /// ID will be composed by the "logical" collectionId and key
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="key"></param>
        /// <returns>Generated document ID</returns>
        public static string GenerateId(string collectionId, string key)
        {
            return $"{collectionId.ToLowerInvariant()}.{key.ToLowerInvariant()}";
        }
    }
}
