using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace MessagingCash
{
    class MessagingSA
    {
        const string connString = "[CONNECTION STRING]";
        CloudStorageAccount storageAcocunt;
        CloudQueueClient queueClient;
        CloudQueue queue;
        public MessagingSA()
        {
            storageAcocunt = CloudStorageAccount.Parse(connString);
            queueClient = storageAcocunt.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference("messages");
            queue.CreateIfNotExistsAsync();
        }
        public async Task AddQueueMessage(int total)
        {
            string msj;
            for (int i = 0; i < total; i++)
            {
                msj = string.Format("Test message {0}", i);
                await queue.AddMessageAsync(new CloudQueueMessage(msj));
                Console.WriteLine(msj);
            }
        }
        public async Task PeekQueueMessage()
        {
            CloudQueueMessage peekMessage = await queue.PeekMessageAsync();
            Console.WriteLine(peekMessage.AsString);
        }
        public async Task GetQueueMessage()
        {
            CloudQueueMessage getMessage = await queue.GetMessageAsync();
            Console.WriteLine(getMessage.AsString);

            //await queue.DeleteMessageAsync(getMessage);
        }
        public async Task UpdateQueueMessage(string message)
        {
            CloudQueueMessage getMessage = await queue.GetMessageAsync();
            getMessage.SetMessageContent(message);
            await queue.UpdateMessageAsync(getMessage, TimeSpan.FromSeconds(5), MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }
    }
}
