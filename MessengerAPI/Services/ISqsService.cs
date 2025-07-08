namespace MessengerAPI.Services
{
    public interface ISqsService
    {
        Task SendMessageToQueueAsync(string queueUrl, string messageBody);
        Task<string> ReceiveMessageFromQueueAsync(string queueUrl);
        Task DeleteMessageFromQueueAsync(string queueUrl, string receiptHandle);
    }
}