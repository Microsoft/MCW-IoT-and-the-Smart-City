// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Models;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime;
using Moq;
using Services.Test.helpers;
using Xunit;

namespace Services.Test
{
    public class DocumentDbKeyValueContainerTest
    {
        private const string MOCK_DB_ID = "mockdb";
        private const string MOCK_COLL_ID = "mockcoll";
        private static readonly string mockCollectionLink = $"/dbs/{MOCK_DB_ID}/colls/{MOCK_COLL_ID}";

        private readonly Mock<IDocumentClient> mockClient;
        private readonly DocumentDbKeyValueContainer container;
        private readonly Random rand = new Random();

        public DocumentDbKeyValueContainerTest()
        {
            this.mockClient = new Mock<IDocumentClient>();

            this.container = new DocumentDbKeyValueContainer(
                new MockFactory<IDocumentClient>(this.mockClient),
                new MockExceptionChecker(),
                new ServicesConfig
                {
                    StorageType = "documentDb",
                    DocumentDbConnString = "",
                    DocumentDbDatabase = MOCK_DB_ID,
                    DocumentDbCollection = MOCK_COLL_ID,
                    DocumentDbRUs = 567
                },
                new Logger("UnitTest", LogLevel.Debug));
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task GetAsyncTest()
        {
            var collectionId = this.rand.NextString();
            var key = this.rand.NextString();
            var data = this.rand.NextString();
            var etag = this.rand.NextString();
            var timestamp = this.rand.NextDateTimeOffset();

            var document = new Document();
            document.SetPropertyValue("CollectionId", collectionId);
            document.SetPropertyValue("Key", key);
            document.SetPropertyValue("Data", data);
            document.SetETag(etag);
            document.SetTimestamp(timestamp);
            var response = new ResourceResponse<Document>(document);

            this.mockClient
                .Setup(x => x.ReadDocumentAsync(
                    It.IsAny<string>(),
                    It.IsAny<RequestOptions>()))
                .ReturnsAsync(response);

            var result = await this.container.GetAsync(collectionId, key);

            Assert.Equal(result.CollectionId, collectionId);
            Assert.Equal(result.Key, key);
            Assert.Equal(result.Data, data);
            Assert.Equal(result.ETag, etag);
            Assert.Equal(result.Timestamp, timestamp);

            this.mockClient
                .Verify(x => x.ReadDocumentAsync(
                        It.Is<string>(s => s == $"{mockCollectionLink}/docs/{collectionId.ToLowerInvariant()}.{key.ToLowerInvariant()}"),
                        It.IsAny<RequestOptions>()),
                    Times.Once);
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task GetAsyncNotFoundTest()
        {
            var collectionId = this.rand.NextString();
            var key = this.rand.NextString();

            this.mockClient
                .Setup(x => x.ReadDocumentAsync(
                    It.IsAny<string>(),
                    It.IsAny<RequestOptions>()))
                .ThrowsAsync(new ResourceNotFoundException());

            await Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
                await this.container.GetAsync(collectionId, key));
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task GetAllAsyncTest()
        {
            var collectionId = this.rand.NextString();
            var documents = new[]
            {
                new KeyValueDocument(collectionId, this.rand.NextString(), this.rand.NextString()),
                new KeyValueDocument(collectionId, this.rand.NextString(), this.rand.NextString()),
                new KeyValueDocument(collectionId, this.rand.NextString(), this.rand.NextString()),
            };
            foreach (var doc in documents)
            {
                doc.SetETag(this.rand.NextString());
                doc.SetTimestamp(this.rand.NextDateTimeOffset());
            }

            this.mockClient
                .Setup(x => x.CreateDocumentQuery<KeyValueDocument>(
                    It.IsAny<string>(),
                    It.IsAny<FeedOptions>()))
                .Returns(documents.AsQueryable().OrderBy(doc => doc.Id));

            var result = (await this.container.GetAllAsync(collectionId)).ToList();

            Assert.Equal(result.Count(), documents.Length);
            foreach (var model in result)
            {
                var doc = documents.Single(d => d.Key == model.Key);
                Assert.Equal(model.CollectionId, collectionId);
                Assert.Equal(model.Data, doc.Data);
                Assert.Equal(model.ETag, doc.ETag);
                Assert.Equal(model.Timestamp, doc.Timestamp);
            }

            this.mockClient
                .Verify(x => x.CreateDocumentQuery<KeyValueDocument>(
                        It.Is<string>(s => s == mockCollectionLink),
                        It.IsAny<FeedOptions>()),
                    Times.Once);
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task CreateAsyncTest()
        {
            var collectionId = this.rand.NextString();
            var key = this.rand.NextString();
            var data = this.rand.NextString();
            var etag = this.rand.NextString();
            var timestamp = this.rand.NextDateTimeOffset();

            var document = new Document();
            document.SetPropertyValue("CollectionId", collectionId);
            document.SetPropertyValue("Key", key);
            document.SetPropertyValue("Data", data);
            document.SetETag(etag);
            document.SetTimestamp(timestamp);
            var response = new ResourceResponse<Document>(document);

            this.mockClient
                .Setup(x => x.CreateDocumentAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(response);

            var result = await this.container.CreateAsync(collectionId, key, new ValueServiceModel
            {
                Data = data
            });

            Assert.Equal(result.CollectionId, collectionId);
            Assert.Equal(result.Key, key);
            Assert.Equal(result.Data, data);
            Assert.Equal(result.ETag, etag);
            Assert.Equal(result.Timestamp, timestamp);

            this.mockClient
                .Verify(x => x.CreateDocumentAsync(
                        It.Is<string>(s => s == mockCollectionLink),
                        It.Is<KeyValueDocument>(doc => doc.Id == $"{collectionId.ToLowerInvariant()}.{key.ToLowerInvariant()}" && doc.CollectionId == collectionId && doc.Key == key && doc.Data == data),
                        It.IsAny<RequestOptions>(),
                        It.IsAny<bool>()),
                    Times.Once);
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task CreateAsyncConflictTest()
        {
            var collectionId = this.rand.NextString();
            var key = this.rand.NextString();
            var data = this.rand.NextString();

            this.mockClient
                .Setup(x => x.CreateDocumentAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>()))
                .ThrowsAsync(new ConflictingResourceException());

            await Assert.ThrowsAsync<ConflictingResourceException>(async () =>
                await this.container.CreateAsync(collectionId, key, new ValueServiceModel
                {
                    Data = data
                }));
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task UpsertAsyncTest()
        {
            var collectionId = this.rand.NextString();
            var key = this.rand.NextString();
            var data = this.rand.NextString();
            var etagOld = this.rand.NextString();
            var etagNew = this.rand.NextString();
            var timestamp = this.rand.NextDateTimeOffset();

            var document = new Document();
            document.SetPropertyValue("CollectionId", collectionId);
            document.SetPropertyValue("Key", key);
            document.SetPropertyValue("Data", data);
            document.SetETag(etagNew);
            document.SetTimestamp(timestamp);
            var response = new ResourceResponse<Document>(document);

            this.mockClient
                .Setup(x => x.UpsertDocumentAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(response);

            var result = await this.container.UpsertAsync(collectionId, key, new ValueServiceModel
            {
                Data = data,
                ETag = etagOld
            });

            Assert.Equal(result.CollectionId, collectionId);
            Assert.Equal(result.Key, key);
            Assert.Equal(result.Data, data);
            Assert.Equal(result.ETag, etagNew);
            Assert.Equal(result.Timestamp, timestamp);

            this.mockClient
                .Verify(x => x.UpsertDocumentAsync(
                        It.Is<string>(s => s == mockCollectionLink),
                        It.Is<KeyValueDocument>(doc => doc.Id == $"{collectionId.ToLowerInvariant()}.{key.ToLowerInvariant()}" && doc.CollectionId == collectionId && doc.Key == key && doc.Data == data),
                        It.IsAny<RequestOptions>(),
                        It.IsAny<bool>()),
                    Times.Once);
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task UpsertAsyncConflictTest()
        {
            var collectionId = this.rand.NextString();
            var key = this.rand.NextString();
            var data = this.rand.NextString();
            var etag = this.rand.NextString();

            this.mockClient
                .Setup(x => x.UpsertDocumentAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>()))
                .ThrowsAsync(new ConflictingResourceException());

            await Assert.ThrowsAsync<ConflictingResourceException>(async () =>
                await this.container.UpsertAsync(collectionId, key, new ValueServiceModel
                {
                    Data = data,
                    ETag = etag
                }));
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task DeleteAsyncTest()
        {
            var collectionId = this.rand.NextString();
            var key = this.rand.NextString();

            this.mockClient
                .Setup(x => x.DeleteDocumentAsync(
                    It.IsAny<string>(),
                    It.IsAny<RequestOptions>()))
                .ReturnsAsync((ResourceResponse<Document>) null);

            await this.container.DeleteAsync(collectionId, key);

            this.mockClient
                .Verify(x => x.DeleteDocumentAsync(
                        It.Is<string>(s => s == $"{mockCollectionLink}/docs/{collectionId.ToLowerInvariant()}.{key.ToLowerInvariant()}"),
                        It.IsAny<RequestOptions>()),
                    Times.Once);
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task DeleteAsyncNotFoundTest()
        {
            var collectionId = this.rand.NextString();
            var key = this.rand.NextString();

            this.mockClient
                .Setup(x => x.DeleteDocumentAsync(
                    It.IsAny<string>(),
                    It.IsAny<RequestOptions>()))
                .ThrowsAsync(new ResourceNotFoundException());

            await this.container.DeleteAsync(collectionId, key);
        }
    }
}
