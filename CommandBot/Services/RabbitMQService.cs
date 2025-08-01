using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MAS_Shared.MASConstants;

namespace CommandBot.Services
{
    public class RabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMQService()
        {
            var factory = new ConnectionFactory() 
            { 
                HostName = MASConstants.MessageQueueHostName
            };
            _connection = factory.CreateConnectionAsync().Result ??
                throw new InvalidOperationException("Failed to create RabbitMQ connection.");
            _channel = _connection.CreateChannelAsync().Result ?? 
                throw new InvalidOperationException("Failed to create RabbitMQ channel.");

            // Declare the queues if they do not exist
            _channel.QueueDeclareAsync(
                queue: MASConstants.CommandQueueName, 
                durable: true, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);

            // Declare additional queues for outbound messages
            _channel.QueueDeclareAsync(
                queue: MASConstants.TelegramOutboundQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclareAsync(
                queue: MASConstants.WhatsAppOutboundQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void PublishCommand(string commandJson)
        {
            var body = Encoding.UTF8.GetBytes(commandJson);
            // Add headers/tags if needed
            var basicProperties = new BasicProperties();

            _channel.BasicPublishAsync(
                exchange: "",
                routingKey: MASConstants.CommandQueueName,
                mandatory: false,
                basicProperties: basicProperties,
                body: body
            );
        }

        public void PublishTelegramOutbound(string json) =>
            _channel.BasicPublishAsync(
                "",
                MASConstants.TelegramOutboundQueue,
                false,
                new BasicProperties(),
                Encoding.UTF8.GetBytes(json));

        public void PublishWhatsAppOutbound(string json) =>
            _channel.BasicPublishAsync(
                "",
                MASConstants.WhatsAppOutboundQueue,
                false,
                new BasicProperties(),
                Encoding.UTF8.GetBytes(json));

        public void PublishOutboundMessage(string outboundJson)
        {
            var body = Encoding.UTF8.GetBytes(outboundJson);
            // Add headers/tags if needed
            var props = new BasicProperties();

            _channel.BasicPublishAsync(
                exchange: "",
                routingKey: MASConstants.TelegramOutboundQueue,
                mandatory: false,
                basicProperties: props,
                body: body
            );
        }

        public void StartConsuming(Action<string> processCommand)
        {
            // Ensure the queue exists before consuming
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    processCommand(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
                await Task.CompletedTask;
            };
            _channel.BasicConsumeAsync(queue: MASConstants.CommandQueueName, autoAck: true, consumer: consumer);
        }

        public void StartConsumingWhatsApp(Action<string> handleWhatsAppDispatch)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    handleWhatsAppDispatch(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WhatsApp dispatch error: {ex.Message}");
                }
                await Task.CompletedTask;
            };

            _channel.BasicConsumeAsync(
                queue: MASConstants.WhatsAppOutboundQueue,
                autoAck: true,
                consumer: consumer
            );
        }

        public void StartConsumingTelegram(Action<string> handleTelegramDispatch)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    // Optional: You can filter here if needed
                    handleTelegramDispatch(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Telegram dispatch error: {ex.Message}");
                }
                await Task.CompletedTask;
            };

            _channel.BasicConsumeAsync(
                queue: MASConstants.TelegramOutboundQueue,
                autoAck: true,
                consumer: consumer
            );
        }
    }
}