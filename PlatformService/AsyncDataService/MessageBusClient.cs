using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataService
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;

        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMqHost"],
                Port = int.Parse(_configuration["RabbitMqPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("---> Connected to RabbitMq Client");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Could not connect to RabbitMq Client {ex.Message}");
            }
        }


        public void PublishNewMessage(PlatformPublishDto platformPublishDto)
        {
            var message = JsonSerializer.Serialize(platformPublishDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("---> RabbitMQ client connection is open ... sending message ...");
                SendMessage(message);

            }
            else
            {
                Console.WriteLine("---> RabbitMq client connection closed message not sending..");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger",
                    routingKey: "",
                    basicProperties: null,
                    body: body);
            Console.WriteLine($"--> We have send {message}");
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMq connection shutdown");
        }

        public void Dispose()
        {
            Console.WriteLine("--> RabbitMQ Disposed");

            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }

}