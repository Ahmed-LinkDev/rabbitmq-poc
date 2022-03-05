using System;
using System.Threading;
using RabbitMQ.Client;

namespace RabbitMQ_Commons
{
    public static class RabbitMqUtilities
    {
        /// <summary>
        /// Initialize connection to RabbitMQ
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="connectionShutdownHandler">The event envoked when connection closed</param>
        /// <returns></returns>
        public static IConnection InitRabbitMqConnection(string hostName, string userName, string password
            , EventHandler<ShutdownEventArgs> connectionShutdownHandler = null)
        {
            IConnection connection = null;
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            var isOpen = false;
            while (!isOpen)
            {
                try
                {
                    factory.AutomaticRecoveryEnabled = true;
                    factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
                    // create connection  
                    connection = factory.CreateConnection();
                    isOpen = true;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
                {
                    //Todo: We shall add logging

                    Thread.Sleep(5000);
                }
            }

            connection.ConnectionShutdown += connectionShutdownHandler;

            return connection;
        }

        /// <summary>
        /// Initiate RabbitMQ channel
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static IModel CreateChannel(IConnection connection, string exchangeName, string queueName)
        {
            IModel channel = connection.CreateModel();

            //Declare the Exchange
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true);
            //Declare the Queue
            channel.QueueDeclare(queueName, false, false, false, null);
            //Bind the Queue to the Exchange
            channel.QueueBind(queueName, exchangeName, "", null);
            channel.BasicQos(0, 1, false);

            return channel;
        }

        /// <summary>
        /// Broadcast a message on the given queue channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="properties"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static bool SendMessage(IModel channel, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            if (channel.IsOpen)
            {
                channel.BasicPublish(
                    exchange: exchange,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body
                );

                return true;
            }
            else
            {
                //Todo: We shall add logging
                return false;
            }
        }
    }
}
