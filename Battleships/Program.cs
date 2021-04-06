using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Battleships
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost webHost = CreateHostBuilder(args).Build();
            await webHost.RunAsync();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True");
                });
    }
}
