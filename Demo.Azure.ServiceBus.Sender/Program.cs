using Azure.Messaging.ServiceBus;

namespace Demo.Azure.ServiceBus.Sender
{
    internal class Program
    {
        private const string ConnectionString = "Endpoint=sb://";
        private const string QueueName = "Demo.DeleteMe.Test.Queue";

        private static readonly List<string> Messages = new();

        static Program()
        {
            IEnumerable<int> range = Enumerable.Range(1, 50);

            foreach (int message in range)
            {
                Messages.Add($"Message: {message}");
            }
        }

        private static async Task Main()
        {
            await using ServiceBusClient client = new(ConnectionString);

            ServiceBusSender sender = client.CreateSender(QueueName);

            foreach (string message in Messages)
            {
                ServiceBusMessage serviceBusMessage = new(message);
                await sender.SendMessageAsync(serviceBusMessage);
                Console.WriteLine($">>> Sent: {message}");
            }

            await sender.CloseAsync();
        }
    }
}