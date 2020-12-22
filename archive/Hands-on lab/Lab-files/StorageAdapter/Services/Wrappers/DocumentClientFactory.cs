// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text.RegularExpressions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Wrappers
{
    public class DocumentClientFactory : IFactory<IDocumentClient>
    {
        private readonly Uri docDbEndpoint;
        private readonly string docDbKey;

        public DocumentClientFactory(IServicesConfig config, ILogger logger)
        {
            var match = Regex.Match(config.DocumentDbConnString, "^AccountEndpoint=(?<endpoint>.*);AccountKey=(?<key>.*);$");
            if (!match.Success)
            {
                var message = "Invalid connection string for Cosmos DB";
                logger.Error(message, () => { });
                throw new InvalidConfigurationException(message);
            }

            this.docDbEndpoint = new Uri(match.Groups["endpoint"].Value);
            this.docDbKey = match.Groups["key"].Value;
        }

        public IDocumentClient Create()
        {
            return new DocumentClient(this.docDbEndpoint, this.docDbKey);
        }
    }
}
