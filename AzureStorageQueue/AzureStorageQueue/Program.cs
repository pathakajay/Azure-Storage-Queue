using System;
using AzureStorageQueue.Operations;
using Microsoft.Extensions.Configuration;

namespace AzureStorageQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            //var builder = new ConfigurationBuilder();
            //builder.AddJsonFile("appsettings.json", true, true);
            BasicQueueOperation.Run().GetAwaiter().GetResult();

            Console.WriteLine("Hello World!");
        }
    }
}
