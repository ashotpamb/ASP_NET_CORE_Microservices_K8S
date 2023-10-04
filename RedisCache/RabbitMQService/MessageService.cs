
using RabbitMQ.Client;
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

            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName,
                    exchange: "trigger",
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

            return Task.CompletedTask;
        }
    }

}