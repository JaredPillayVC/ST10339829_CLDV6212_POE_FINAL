using Azure.Storage.Queues;

namespace ST10339829_CLDV6212_POE_FINAL.Services
{
    public class AzureQueueService
    {
        private readonly QueueClient _client;

        public AzureQueueService(string connectionString)
        {
            _client = new QueueClient(connectionString, "process-order");
            _client.CreateIfNotExists();
        }
        public async Task CreateMessageAsync(string message)
        {
            await _client.SendMessageAsync(message);
        }
        public async Task<List<string>> RetriveMessagesAsync()
        {
            var message = new List<string>();
            var response = await _client.ReceiveMessagesAsync(maxMessages: 7);
            foreach (var item in response.Value)
            {
                message.Add(item.MessageText);
                await _client.DeleteMessageAsync(item.MessageId, item.PopReceipt);
            }
            return message;
        }
    }
}
