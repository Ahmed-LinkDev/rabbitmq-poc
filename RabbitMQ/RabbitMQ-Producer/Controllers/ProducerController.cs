using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ_Commons;

namespace RabbitMQ_Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly ILogger<ProducerController> _logger;
        private static IModel _channel;

        public ProducerController(ILogger<ProducerController> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = EnvironmentVariables.RabbitMqHostDocker,
                UserName = EnvironmentVariables.RabbitMqUserName,
                Password = EnvironmentVariables.RabbitMqPassword
            };
            try
            {
                var connection = factory.CreateConnection();
                _channel ??= connection.CreateModel();
            }
            catch (Exception exc)
            {
                _logger.LogError(
                    $"Couldn't establish connection to RabbitMQ host '{EnvironmentVariables.RabbitMqHost}'. Error:" +
                    exc.Message);
            }
        }

        [HttpPost("{myMessage}")]
        public IActionResult Send([FromQuery]string myMessage)
        {
            var body = Encoding.UTF8.GetBytes(myMessage);
            var props = _channel.CreateBasicProperties();
            var exchange = EnvironmentVariables.QueueName;
            if (RabbitMqUtilities.SendMessage(_channel, exchange, "", props, body))
                return Ok();
            else
                return UnprocessableEntity($"There is an error occured while sending message '{myMessage}'.");
        }


    }
}