// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Wrappers;
using Moq;

namespace Services.Test.helpers
{
    public class MockFactory<T> : IFactory<T> where T : class
    {
        private readonly Mock<T> mock;

        public MockFactory(Mock<T> mock)
        {
            this.mock = mock;
        }

        public T Create()
        {
            return this.mock.Object;
        }
    }
}
