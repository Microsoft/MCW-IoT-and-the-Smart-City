// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.v1.Filters;
using Moq;
using WebService.Test.helpers;
using Xunit;

namespace WebService.Test.v1.Filters
{
    public class ExceptionsFilterAttributeTest
    {
        private readonly ExceptionsFilterAttribute target;
        private readonly Mock<ILogger> logger;

        public ExceptionsFilterAttributeTest()
        {
            this.logger = new Mock<ILogger>();
            this.target = new ExceptionsFilterAttribute(this.logger.Object);
        }

        /// <summary>
        /// When handling unknown/unexpected exceptions, the stack trace could be null,
        /// the filter must support this scenario.
        /// </summary>
        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public void Doesnt_Fail_When_StackTraces_AreNull()
        {
            // Arrange
            var internalException = new Mock<Exception>();
            var exception = new Exception("", internalException.Object);
            internalException.SetupGet(x => x.StackTrace).Returns((string) null);

            var context = new ExceptionContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(),
                    new ActionDescriptor(),
                    new ModelStateDictionary()),
                new List<IFilterMetadata>()) { Exception = exception };

            // Act
            this.target.OnException(context);

            // Assert
            var result = (ObjectResult) context.Result;
            Assert.Equal((int) HttpStatusCode.InternalServerError, result.StatusCode.Value);

            var content = (Dictionary<string, object>) result.Value;
            Assert.True(content.ContainsKey("StackTrace"));
            Assert.True(content.ContainsKey("InnerExceptionStackTrace"));
            Assert.Null(content["StackTrace"]);
            Assert.Null(content["InnerExceptionStackTrace"]);
        }
    }
}
