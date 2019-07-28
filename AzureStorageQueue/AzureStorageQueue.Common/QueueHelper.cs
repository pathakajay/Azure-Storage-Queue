using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace AzureStorageQueue.Common
{
    public class QueueHelper
    {

        public static CloudQueueClient GetQueueClient(string connectionString)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);

            // Create the queue client.
            CloudQueueClient client = account.CreateCloudQueueClient();
            return client;
        }

        public static async Task<CloudQueue> GetCloudQueue()
        {

            var cloudQueueClient = GetQueueClient(ConfigurationHanlder.AppOptions().ConnectionString);
            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference(ConfigurationHanlder.AppOptions().QueueName);
            bool createdQueue = await cloudQueue.CreateIfNotExistsAsync();
            return cloudQueue;
        }
    }
}
