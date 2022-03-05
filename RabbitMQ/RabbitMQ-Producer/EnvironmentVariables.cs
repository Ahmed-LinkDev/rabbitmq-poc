using System;

namespace RabbitMQ_Producer
{
    public static class EnvironmentVariables
    {
        public static string Get(string variable) => Environment.GetEnvironmentVariable(variable);

        public static readonly string RabbitMqUserName = Get("RABBITMQ_USERNAME");
        public static readonly string RabbitMqPassword = Get("RABBITMQ_PASSWORD");
        public static readonly string QueueName = Get("QUEUE_NAME");
        public static readonly string RabbitMqHostLocal = Get("RABBITMQ_HOST_LOCAL");
        public static readonly string RabbitMqHostDocker = Get("RABBITMQ_HOST_DOCKER");
        public static  string RabbitMqHost => Get("RabbitMqHost");
    }
}