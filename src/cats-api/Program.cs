using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace cats_api
{
    public class Program
    {
        static IConfigurationBuilder configBuilder = new ConfigurationBuilder();
        public static void Main(string[] args)
        {
            configBuilder.Add(new AwsSecretManagerConfigurationSource());
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(configBuilder.Build())
            .UseStartup<Startup>();
    }
}
