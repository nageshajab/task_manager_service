// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TodoListService
{
    public class Program
    {
        static string certPassword;
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Access the configuration
            var config = host.Services.GetService(typeof(IConfiguration)) as IConfiguration;
            certPassword = config["cert_PASSWORD"];

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.ListenAnyIP(80); // HTTP
                        serverOptions.ListenAnyIP(443, listenOptions =>
                        {
                            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                            
                            listenOptions.UseHttps("/https/aspnetapp.pfx", certPassword);
                        }); // HTTPS
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
