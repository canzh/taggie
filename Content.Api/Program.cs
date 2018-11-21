using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Content.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var seed = args.Contains("--seed");
            if (seed)
            {
                args = args.Except(new[] { "--seed" }).ToArray();
            }

            var host = CreateWebHostBuilder(args).Build();

            if (seed)
            {
                await RedisSeed.SeedAsync(host.Services);
            }

            if (args.Contains("--exit"))
            {
                return;
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
