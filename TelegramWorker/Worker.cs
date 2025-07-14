using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

public class Worker : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly string _token = "7978828104:AAFLi0ZYkZ6AU0d7Yq4jcb_j9q7ZEhBinFc";
    private readonly long _chatId = 850099427;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botClient = new TelegramBotClient(_token);

        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "new_message_queue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Nova mensagem recebida: " + message);

            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"🔔 Nova mensagem recebida:\n\n{message}",
                parseMode: ParseMode.Html,
                cancellationToken: stoppingToken);
        };

        channel.BasicConsume(queue: "new_message_queue", autoAck: true, consumer);
        Console.WriteLine("Aguardando mensagens...");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}