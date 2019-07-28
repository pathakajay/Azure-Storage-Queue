using System;
using System.Globalization;
using System.Threading.Tasks;
using AzureStorageQueue.Common;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace AzureStorageQueue.Operations
{
    public class BasicQueueOperation
    {
        const int MessageCount = 5;

        public static async Task Run()
        {

            await AddMessage();
            await UpdateQueueMessage();
            await CreateQueue();
            await DequeueQueue();

            Console.WriteLine("Press a key to exit.");
            Console.ReadLine();
        }
        private static async Task AddMessage()
        {

            CloudQueue queue = await QueueHelper.GetCloudQueue();

            for (int i = 0; i < MessageCount; i++)
            {
                var content = "Hello World" + "\t" + i;
                await queue.AddMessageAsync(new CloudQueueMessage(content));
            }
            Console.WriteLine("Peek at the next message");
            CloudQueueMessage queueMessage = await queue.PeekMessageAsync();
            if (queueMessage != null)
            {
                Console.WriteLine("The peeked message is: {0}", queueMessage.AsString);
            }

            Console.WriteLine("De-queue the next message");
            CloudQueueMessage cloudQueueMessage = await queue.GetMessageAsync();
            if (cloudQueueMessage != null)
            {
                Console.WriteLine("Processing & deleting message with content: {0}", cloudQueueMessage.AsString);
                await queue.DeleteMessageAsync(cloudQueueMessage);
            }
        }
        public static async Task<bool> CreateQueue()
        {
            string queueName = "demo-test-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                              .Replace(" ", "")
                              .Replace(":", "")
                              .Replace(".", "");
            var connectionString = ConfigurationHanlder.AppOptions().ConnectionString;
            var client = QueueHelper.GetQueueClient(connectionString);

            // Retrieve a reference to a queue.
            CloudQueue queue = client.GetQueueReference(queueName);

            try
            {
                // Create the queue if it doesn't already exist.
                await queue.CreateIfNotExistsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return true;

        }
        public static async Task UpdateQueueMessage()
        {

            CloudQueue queue = await QueueHelper.GetCloudQueue();


            CloudQueueMessage cloudQueueMessage = await queue.GetMessageAsync();

            if (cloudQueueMessage != null)
            {
                cloudQueueMessage.SetMessageContent2(cloudQueueMessage.AsString + "\t" + "Update Message", false);
                await queue.UpdateMessageAsync(cloudQueueMessage, TimeSpan.Zero,
                    MessageUpdateFields.Content | MessageUpdateFields.Visibility);


            }
        }

        public static async Task DequeueQueue()
        {

            CloudQueue queue = await QueueHelper.GetCloudQueue();

            Console.WriteLine("Get the queue length");

            await queue.FetchAttributesAsync();
            int? cachedMessageCount = queue.ApproximateMessageCount;
            Console.WriteLine("Number of messages in queue: {0}", cachedMessageCount);
            QueueRequestOptions options = new QueueRequestOptions();
            OperationContext operationContext = new OperationContext();

            if (cachedMessageCount == null)
            {
                Console.WriteLine("No Message Present in Queue");
                return;
            }

            foreach (CloudQueueMessage message in await queue.GetMessagesAsync(cachedMessageCount.Value,
                new TimeSpan(0, 0, 5, 0), options, operationContext))
            {
                // Process all messages in less than 5 minutes, deleting each message after processing.

                Console.WriteLine("Processing {0} Message", message.AsString);
                await queue.DeleteMessageAsync(message);
            }
        }
    }
}
