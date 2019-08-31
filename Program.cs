using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BotWithAPI
{
	public class Program
	{
		public static void Main(string[] args) { CreateWebHostBuilder(args).Build().Run(); }

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
							.ConfigureAppConfiguration((hostBuilder, configBuilder)
																	=> configBuilder.AddJsonFile("appsettings.json",false)
																		.AddJsonFile($"appsettings.{hostBuilder.HostingEnvironment.EnvironmentName}.json", true)
																		.AddJsonEnvVar("QUICKSTART_SETTINGS", true))
							.UseStartup<Startup>();
		}
	}
}
