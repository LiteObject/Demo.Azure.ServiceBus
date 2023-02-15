using Azure.Messaging.ServiceBus;

namespace Demo.Azure.ServiceBus.Receiver
{
    internal class Program
    {
        private const string ConnectionString = "Endpoint=sb://";
        private const string QueueName = "Demo.DeleteMe.Test.Queue";

        private static async Task Main(string[] args)
        {
            // Create a service bus client
            await using ServiceBusClient client = new(ConnectionString);

            // Create a service bus receiver
            ServiceBusReceiver receiver = client.CreateReceiver(QueueName);

            while (true)
            {
                ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

                if (message is not null)
                {
                    Console.WriteLine(message.Body.ToString());
                    await receiver.CompleteMessageAsync(message);
                }
                else
                {
                    Console.WriteLine("\nAll messages received.");
                    break;
                }
            }

            // Close receiver
            await receiver.CloseAsync();
        }
    }
}