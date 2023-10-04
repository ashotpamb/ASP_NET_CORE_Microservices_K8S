
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisCacahe.EventProcessing;

namespace RedisCacahe.RabbitMQService
{
    public class MessageService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessing _eventProcessing;

        private IConnection _connection;

        private IModel _channel;

        private string _queueName;

        public MessageService(IConfiguration configuration, IEventProcessing eventProcessing)
        {
            _configuration = configuration;
            _eventProcessing = eventProcessing;
            InitializeRabbitMQ();
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

            _channel.ExchangeDeclare(exchange: "redis", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName,
                    exchange: "redis",
                    routingKey: ""
                    );

            Console.WriteLine("--> Listening on the Message bus");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
        }

        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Connection shut down");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event received");
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessing.EventProcess(notificationMessage);
            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }

}