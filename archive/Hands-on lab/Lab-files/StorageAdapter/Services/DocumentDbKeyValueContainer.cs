// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Helpers;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Models;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Wrappers;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.Services
{
    public sealed class DocumentDbKeyValueContainer : IKeyValueContainer, IDisposable
    {
        private readonly IDocumentClient client;
        private readonly IExceptionChecker exceptionChecker;
        private readonly ILogger log;

        private readonly string docDbDatabase;
        private readonly string docDbCollection;
        private readonly int docDbRUs;
        private readonly RequestOptions docDbOptions;
        private bool disposedValue;
        private string collectionLink;

        public DocumentDbKeyValueContainer(
            IFactory<IDocumentClient> clientFactory,
            IExceptionChecker exceptionChecker,
            IServicesConfig config,
            ILogger logger)
        {
            this.disposedValue = false;

            this.client = clientFactory.Create();
            this.exceptionChecker = exceptionChecker;
            this.log = logger;

            this.docDbDatabase = config.DocumentDbDatabase;
            this.docDbCollection = config.DocumentDbCollection;
            this.docDbRUs = config.DocumentDbRUs;
            this.docDbOptions = this.GetDocDbOptions();
        }

        public async Task<StatusResultServiceModel> PingAsync()
        {
            var result = new StatusResultServiceModel(false, "Storage check failed");

            try
            {
                DatabaseAccount response = null;
                if (this.client != null)
                {
                    // make generic call to see if storage client can be reached
                    response = await this.client.GetDatabaseAccountAsync();
                }

                if (response != null)
                {
                    result.IsHealthy = true;
                    result.Message = "Alive and Well!";
                }
            }
            catch (Exception e)
            {
                this.log.Info(result.Message, () => new { e });
            }

            return result;
        }

        public async Task<ValueServiceModel> GetAsync(string collectionId, string key)
        {
            await this.SetupStorageAsync();

            try
            {
                var docId = DocumentIdHelper.GenerateId(collectionId, key);
                var response = await this.client.ReadDocumentAsync($"{this.collectionLink}/docs/{docId}");
                return new ValueServiceModel(response);
            }
            catch (Exception ex)
            {
                if (!this.exceptionChecker.IsNotFoundException(ex)) throw;

                const string message = "The resource requested doesn't exist.";
                this.log.Info(message, () => new
                {
                    collectionId,
                    key
                });

                throw new ResourceNotFoundException(message);
            }
        }

        public async Task<IEnumerable<ValueServiceModel>> GetAllAsync(string collectionId)
        {
            await this.SetupStorageAsync();

            var query = this.client.CreateDocumentQuery<KeyValueDocument>(this.collectionLink)
                .Where(doc => doc.CollectionId.ToLower() == collectionId.ToLower())
                .ToList();
            return await Task.FromResult(query.Select(doc => new ValueServiceModel(doc)));
        }

        public async Task<ValueServiceModel> CreateAsync(string collectionId, string key, ValueServiceModel input)
        {
            await this.SetupStorageAsync();

            try
            {
                var response = await this.client.CreateDocumentAsync(
                    this.collectionLink,
                    new KeyValueDocument(collectionId, key, input.Data));
                return new ValueServiceModel(response);
            }
            catch (Exception ex)
            {
                if (!this.exceptionChecker.IsConflictException(ex)) throw;

                const string message = "There is already a value with the key specified.";
                this.log.Info(message, () => new { collectionId, key });
                throw new ConflictingResourceException(message);
            }
        }

        public async Task<ValueServiceModel> UpsertAsync(string collectionId, string key, ValueServiceModel input)
        {
            await this.SetupStorageAsync();

            try
            {
                var response = await this.client.UpsertDocumentAsync(
                    this.collectionLink,
                    new KeyValueDocument(collectionId, key, input.Data),
                    IfMatch(input.ETag));
                return new ValueServiceModel(response);
            }
            catch (Exception ex)
            {
                if (!this.exceptionChecker.IsPreconditionFailedException(ex)) throw;

                const string message = "ETag mismatch: the resource has been updated by another client.";
                this.log.Info(message, () => new { collectionId, key, input.ETag });
                throw new ConflictingResourceException(message);
            }
        }

        public async Task DeleteAsync(string collectionId, string key)
        {
            await this.SetupStorageAsync();

            try
            {
                await this.client.DeleteDocumentAsync($"{this.collectionLink}/docs/{DocumentIdHelper.GenerateId(collectionId, key)}");
            }
            catch (Exception ex)
            {
                if (!this.exceptionChecker.IsNotFoundException(ex)) throw;

                this.log.Debug("Key does not exist, nothing to do", () => new { key });
            }
        }

        private RequestOptions GetDocDbOptions()
        {
            return new RequestOptions
            {
                OfferThroughput = this.docDbRUs,
                ConsistencyLevel = ConsistencyLevel.Strong
            };
        }

        private async Task SetupStorageAsync()
        {
            if (string.IsNullOrEmpty(this.collectionLink))
            {
                await this.CreateDatabaseIfNotExistsAsync();
                await this.CreateCollectionIfNotExistsAsync();
                this.collectionLink = $"/dbs/{this.docDbDatabase}/colls/{this.docDbCollection}";
            }
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                var uri = "/dbs/" + this.docDbDatabase;
                await this.client.ReadDatabaseAsync(uri, this.docDbOptions);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                {
                    this.log.Error("Error while getting DocumentDb database", () => new { e });
                }

                await this.CreateDatabaseAsync();
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                var uri = $"/dbs/{this.docDbDatabase}/colls/{this.docDbCollection}";
                await this.client.ReadDocumentCollectionAsync(uri, this.docDbOptions);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                {
                    this.log.Error("Error while getting DocumentDb collection", () => new { e });
                }

                await this.CreateCollectionAsync();
            }
        }

        private async Task CreateDatabaseAsync()
        {
            try
            {
                this.log.Info("Creating DocumentDb database",
                    () => new { this.docDbDatabase });
                var db = new Database { Id = this.docDbDatabase };
                await this.client.CreateDatabaseAsync(db);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.Conflict)
                {
                    this.log.Warn("Another process already created the database",
                        () => new { this.docDbDatabase });
                }

                this.log.Error("Error while creating DocumentDb database",
                    () => new { this.docDbDatabase, e });
            }
            catch (Exception e)
            {
                this.log.Error("Error while creating DocumentDb database",
                    () => new { this.docDbDatabase, e });
                throw;
            }
        }

        private async Task CreateCollectionAsync()
        {
            try
            {
                this.log.Info("Creating DocumentDb collection",
                    () => new { this.docDbCollection });
                var coll = new DocumentCollection { Id = this.docDbCollection };

                var index = Index.Range(DataType.String, -1);
                var indexing = new IndexingPolicy(index) { IndexingMode = IndexingMode.Consistent };
                coll.IndexingPolicy = indexing;

                // Partitioning can be enabled in case the storage adapter is used to store 100k+ records
                //coll.PartitionKey = new PartitionKeyDefinition { Paths = new Collection<string> { "/CollectionId" } };

                var dbUri = "/dbs/" + this.docDbDatabase;
                await this.client.CreateDocumentCollectionAsync(dbUri, coll, this.docDbOptions);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.Conflict)
                {
                    this.log.Warn("Another process already created the collection",
                        () => new { this.docDbCollection });
                }

                this.log.Error("Error while creating DocumentDb collection",
                    () => new { this.docDbCollection, e });
            }
            catch (Exception e)
            {
                this.log.Error("Error while creating DocumentDb collection",
                    () => new { this.docDbDatabase, e });
                throw;
            }
        }

        private static RequestOptions IfMatch(string etag)
        {
            if (etag == "*")
            {
                // Match all
                return null;
            }
            return new RequestOptions
            {
                AccessCondition = new AccessCondition
                {
                    Condition = etag,
                    Type = AccessConditionType.IfMatch
                }
            };
        }

        #region IDisposable Support

        private void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    (this.client as IDisposable)?.Dispose();
                }
                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion
    }
}
