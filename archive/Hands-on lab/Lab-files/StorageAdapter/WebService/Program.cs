// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime;
using Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.Runtime;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.WebService
{
    /// <summary>Application entry point</summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new Config(new ConfigData(new Logger(Uptime.ProcessId, LogLevel.Info)));

            /*
            Print some information to help development and debugging, like
            runtime and configuration settings
            */
            Console.WriteLine($"[{Uptime.ProcessId}] Starting web service started, process ID: " + Uptime.ProcessId);
            Console.WriteLine($"[{Uptime.ProcessId}] Web service listening on port " + config.Port);
            Console.WriteLine($"[{Uptime.ProcessId}] Web service health check at: http://127.0.0.1:" + config.Port + "/" + v1.Version.PATH + "/status");

            /*
            Kestrel is a cross-platform HTTP server based on libuv, a
            cross-platform asynchronous I/O library.
            https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers
            */
            var host = new WebHostBuilder()
                .UseUrls("http://*:" + config.Port)
                .UseKestrel(options => { options.AddServerHeader = false; })
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
