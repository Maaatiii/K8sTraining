using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace TestWebApplication
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.MinimumLevel.Debug()
				.WriteTo.Console(
					LogEventLevel.Verbose,
					"{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
				.WriteTo.Seq("http://halting-olm-seq.default.svc.cluster.local:5341")
					.Enrich.FromLogContext()
					.Enrich.WithThreadId()
					.Enrich.WithAssemblyInformationalVersion()
					.Enrich.WithUserName()
					.Enrich.WithMemoryUsage()
				.CreateLogger();

			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					// ..  
					config.AddEnvironmentVariables(); // <---
					// ..
				})
				.UseSerilog()
				.UseStartup<Startup>();
	}
}
