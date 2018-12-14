﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ProxyKit.Examples
{
    public abstract class ExampleBase<T> where T : class
    {
        public void Run(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                await WebHost.CreateDefaultBuilder<T>(Array.Empty<string>())
                    .UseUrls("http://localhost:5000")
                    .Build()
                    .RunAsync(cancellationToken);
            }).GetAwaiter().GetResult();
        }
    }
}