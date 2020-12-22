// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.Runtime
{
    public interface IConfig
    {
        /// <summary>Web service listening port</summary>
        int Port { get; }

        /// <summary>Service layer configuration</summary>
        IServicesConfig ServicesConfig { get; }
    }

    /// <summary>Web service configuration</summary>
    public class Config : IConfig
    {
        private const string APPLICATION_KEY = "StorageAdapter:";
        private const string PORT_KEY = APPLICATION_KEY + "webservicePort";
        private const string STORAGE_TYPE_KEY = APPLICATION_KEY + "storageType";
        private const string DOCUMENT_DB_CONNECTION_STRING_KEY = APPLICATION_KEY + "documentDBConnectionString";
        private const string DOCUMENT_DB_DATABASE_KEY = APPLICATION_KEY + "documentDBdatabase";
        private const string DOCUMENT_DB_COLLECTION_KEY = APPLICATION_KEY + "documentDBcollection";
        private const string DOCUMENT_DB_RUS_KEY = APPLICATION_KEY + "documentDBRUs";

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig { get; }

        public Config(IConfigData configData)
        {
            this.Port = configData.GetInt(PORT_KEY);

            var storageType = configData.GetString(STORAGE_TYPE_KEY).ToLowerInvariant();
            var documentDbConnString = configData.GetString(DOCUMENT_DB_CONNECTION_STRING_KEY);
            if (storageType == "documentdb" &&
                (string.IsNullOrEmpty(documentDbConnString)
                 || documentDbConnString.StartsWith("${")
                 || documentDbConnString.Contains("...")))
            {
                // In order to connect to the storage, the service requires a connection
                // string for Document Db. The value can be found in the Azure Portal.
                // The connection string can be stored in the 'appsettings.ini' configuration
                // file, or in the PCS_STORAGEADAPTER_DOCUMENTDB_CONNSTRING environment variable.
                // When working with VisualStudio, the environment variable can be set in the
                // WebService project settings, under the "Debug" tab.
                throw new Exception("The service configuration is incomplete. " +
                                    "Please provide your DocumentDb connection string. " +
                                    "For more information, see the environment variables " +
                                    "used in project properties and the 'documentdb_connstring' " +
                                    "value in the 'appsettings.ini' configuration file.");
            }

            this.ServicesConfig = new ServicesConfig
            {
                StorageType = storageType,
                DocumentDbConnString = documentDbConnString,
                DocumentDbDatabase = configData.GetString(DOCUMENT_DB_DATABASE_KEY),
                DocumentDbCollection = configData.GetString(DOCUMENT_DB_COLLECTION_KEY),
                DocumentDbRUs = configData.GetInt(DOCUMENT_DB_RUS_KEY),
            };
        }
    }
}
