namespace MessengerAPI.Services
{
    public interface IRabbitMqService
    {
        void PublishMessage(string message);
    }
}