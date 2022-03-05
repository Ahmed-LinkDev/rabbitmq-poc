using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace RabbitMQ_Producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
