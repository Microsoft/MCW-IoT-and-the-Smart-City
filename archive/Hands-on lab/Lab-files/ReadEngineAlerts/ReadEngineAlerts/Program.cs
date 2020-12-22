using System;
using System.IO;
using System.Text;
using Microsoft.ServiceBus.Messaging;

namespace ReadEngineAlerts
{
    class Program
    {
        static void Main(string[] args)
        {
            var count = 1;
            ConsoleColor[] colors = { ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Magenta, ConsoleColor.Yellow };
            Console.WriteLine("Receive critical messages. Ctrl-C to exit.\n");
            var connectionString = "{YOUR-CONNECTION-STRING}";
            var queueName = "alert-q";

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            client.OnMessage(message =>
            {
                var stream = message.GetBody<Stream>();
                var reader = new StreamReader(stream, Encoding.ASCII);
                var s = reader.ReadToEnd();
                Console.ForegroundColor = colors[count % 5];
                Console.WriteLine($"Received > {s}");
                count++;
            });

            Console.ReadLine();
        }

    }
}
