
using System.Text;

using System.Text.Json;
using CommandsService.Dtos;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessing _eventProcessing;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessing eventProcessing)
        {
            _configuration = configuration;
            _eventProcessing = eventProcessing;
            InitializeRabbitMQ();

        }
        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection ShutDown");
        }
        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMqHost"],
                Port = int.Parse(_configuration["RabbitMqPort"])
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName,
                    exchange: "trigger",
                    routingKey: ""
                    );

            Console.WriteLine("--> Listening on the Message bus");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event Received!");
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessing.ProcessEvent(notificationMessage);

                try
                {
                    var commandReadDtos =  new CommandReadDtos() {Id = 2, HowTo ="hello", CommandLine="World", PlatfromId = 5};
                    commandReadDtos.Event = "Cache_in";
                    var jsonData = JsonSerializer.Serialize(commandReadDtos);

                    RedisCache(jsonData, "redis-cache");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not send message to RabbitMQ: {ex.Message}");
                }
            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;

        }

        private void RedisCache(string message, string queuename)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.ExchangeDeclare(exchange: "redis", type: ExchangeType.Fanout);


            _channel.BasicPublish(exchange: "redis",
                    routingKey: "",
                    basicProperties: null,
                    body: body);

            Console.WriteLine($"--> Message sent to queue '{queuename}': {message}");

        }
    }
}