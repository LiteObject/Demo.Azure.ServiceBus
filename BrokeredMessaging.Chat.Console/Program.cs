using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace BrokeredMessaging.Chat.Console
{
    internal class Program
    {
        private const string ConnectionString = "Endpoint=sb://";
        private const string TopicName = "Demo.DeleteMe.Test.Topic";

        private static async Task Main()
        {
            string userName = GetUsernameOrExit();

            // To manage artifacts
            ServiceBusAdministrationClient serviceBusAdministrationClient = new(ConnectionString);

            if (!await serviceBusAdministrationClient.TopicExistsAsync(TopicName))
            {
                _ = await serviceBusAdministrationClient.CreateTopicAsync(TopicName);
            }

            if (!await serviceBusAdministrationClient.SubscriptionExistsAsync(TopicName, userName))
            {
                CreateSubscriptionOptions option = new(TopicName, userName)
                {
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(5),
                };

                _ = await serviceBusAdministrationClient.CreateSubscriptionAsync(option);
            }

            await using ServiceBusClient serviceBusClient = new(ConnectionString);

            await using ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(TopicName);

            await using ServiceBusProcessor serviceBusProcessor = serviceBusClient.CreateProcessor(TopicName, userName);

            serviceBusProcessor.ProcessMessageAsync += MessageHandler;

            serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

            await serviceBusProcessor.StartProcessingAsync();

            ServiceBusMessage serviceBusMessage = new($"[{DateTime.Now}] \"{userName}\" has joined the chat.");
            await serviceBusSender.SendMessageAsync(serviceBusMessage);

            while (true)
            {
                string? message = System.Console.ReadLine();

                if (message != null && message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                serviceBusMessage = new($"[{DateTime.Now}] {userName}: {message}");
                await serviceBusSender.SendMessageAsync(serviceBusMessage);
            }

            serviceBusMessage = new($"[{DateTime.Now}] \"{userName}\" has left the chat.");
            await serviceBusSender.SendMessageAsync(serviceBusMessage);

            await serviceBusProcessor.StopProcessingAsync();

            // await serviceBusProcessor.CloseAsync();
            // await serviceBusSender.CloseAsync();
        }


        private static Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Print(arg.ErrorSource.ToString(), ConsoleColor.Red);
            Print(arg.FullyQualifiedNamespace, ConsoleColor.Red);
            Print(arg.EntityPath, ConsoleColor.Red);
            Print(arg.Exception.ToString(), ConsoleColor.Red);

            return Task.CompletedTask;
        }

        private static async Task MessageHandler(ProcessMessageEventArgs arg)
        {
            string receivedMessage = arg.Message.Body.ToString();
            Print(receivedMessage, ConsoleColor.Cyan);

            await arg.CompleteMessageAsync(arg.Message);
        }

        private static string GetUsernameOrExit()
        {
            string userPrompt = "Enter your name (or type \"exit\" to leave):";
            Print(userPrompt, ConsoleColor.DarkCyan);
            string? userName = System.Console.ReadLine();

            while (string.IsNullOrWhiteSpace(userName))
            {
                Print($"Name can't be empty. {userPrompt}", ConsoleColor.DarkCyan);
                userName = System.Console.ReadLine();
            }

            CheckForExit(userName);

            return userName;
        }

        private static void CheckForExit(string inputText)
        {
            if (inputText.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Print("You are about to exit the application.", ConsoleColor.DarkYellow);
                Environment.Exit(0);
            }
        }

        private static void Print(string value, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            System.Console.ForegroundColor = foregroundColor;
            System.Console.WriteLine(value);
            System.Console.ResetColor();
        }
    }
}