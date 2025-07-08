using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;

namespace MessengerAPI.Services
{
    public class SqsService : ISqsService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly string _awsRegion;
        private readonly string _accessKey;
        private readonly string _secretKey;

        public SqsService(IConfiguration configuration)
        {
            var awsSection = configuration.GetSection("AWS");
            _awsRegion = awsSection["Region"]!;
            _accessKey = awsSection["AccessKey"]!;
            _secretKey = awsSection["SecretKey"]!;

            var sqsConfig = new AmazonSQSConfig { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_awsRegion) };
            _sqsClient = new AmazonSQSClient(_accessKey, _secretKey, sqsConfig);
        }

        public async Task SendMessageToQueueAsync(string queueUrl, string messageBody)
        {
            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody
            };

            await _sqsClient.SendMessageAsync(request);
        }

        public async Task<string> ReceiveMessageFromQueueAsync(string queueUrl)
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 5
            };

            var response = await _sqsClient.ReceiveMessageAsync(request);

            if (response.Messages.Count == 0)
                return null!;

            var message = response.Messages[0];
            return message.Body;
        }

        public async Task DeleteMessageFromQueueAsync(string queueUrl, string receiptHandle)
        {
            var request = new DeleteMessageRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = receiptHandle
            };

            await _sqsClient.DeleteMessageAsync(request);
        }
    }
}