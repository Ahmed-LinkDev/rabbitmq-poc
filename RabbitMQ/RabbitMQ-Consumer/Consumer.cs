using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Commons;


namespace RabbitMQ_Consumer
{
    public class Consumer : BackgroundService
    {
        private readonly ILogger<Consumer> _logger;
        private IConnection _connection;
        private static IModel _channel;

        public Consumer(ILogger<Consumer> logger)
        {
            _logger = logger;
            _connection = RabbitMqUtilities.InitRabbitMqConnection(EnvironmentVariables.RabbitMqHostDocker
                , EnvironmentVariables.RabbitMqUserName
                , EnvironmentVariables.RabbitMqPassword, OnConnectionShutdown);
            if (_channel == null)
            {
                _channel = _connection.CreateModel();
            }
        }       

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.Span);

                // handle the received message  
                // await HandleMessage(content);
                _logger.LogInformation($"Consumer (1) received message: '{content}'");
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(EnvironmentVariables.QueueName, false, consumer);
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning("RabbitMQ Connection Is Down");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}