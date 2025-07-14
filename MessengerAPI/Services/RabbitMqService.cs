using System.Text;
using RabbitMQ.Client;

namespace MessengerAPI.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IModel _channel;

        public RabbitMqService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(queue: "new_message_queue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: "new_message_queue",
                                  basicProperties: null,
                                  body: body);
        }
    }
}