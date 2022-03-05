using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ_Commons;

namespace RabbitMQ_Producer
{
    public class Startup
    {
        private ILogger<Startup> _logger;
        private static IModel _channel;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            _logger = logger;
            IConnection connection = RabbitMqUtilities.InitRabbitMqConnection(EnvironmentVariables.RabbitMqHostDocker
                , EnvironmentVariables.RabbitMqUserName
                , EnvironmentVariables.RabbitMqPassword, OnConnectionShutdown);

            if (_channel == null)
                RabbitMqUtilities.CreateChannel(connection, EnvironmentVariables.QueueName, EnvironmentVariables.QueueName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning("RabbitMQ Connection Is Down");
        }
    }
}