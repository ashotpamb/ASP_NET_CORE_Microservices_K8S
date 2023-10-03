
using System.Text;
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
            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;

        }
    }
}