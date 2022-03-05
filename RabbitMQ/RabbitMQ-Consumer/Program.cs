using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RabbitMQ_Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Thread.Sleep(1000);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;
                    if(env.IsDevelopment())
                        DotNetEnv.Env.Load("..//.env");
                    else
                        DotNetEnv.Env.Load();
                    
                })
                .ConfigureServices((hostContext, services) =>
                {
                    
                    services.AddHostedService<Consumer>();
                });
    }
}