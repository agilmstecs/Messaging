using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using System.Threading.Tasks;
using System.Threading;

namespace MessagingCash
{
    class MessagingSB
    {
        #region Service Bus Queue
        const string ConnStr = "[CONNECTION STRING]";
        const string Queue = "[QUEUE NAME]";

        static IQueueClient queueClient;

        public  MessagingSB()
        {
            queueClient = new QueueClient(ConnStr, Queue);
        }
        public async Task SetMessagesAsync(int totalMessages)
        {
            Message message;
            try
            {
                for (int i = 0; i < totalMessages; i++)
                {
                    var msj = $"Message: {i}";
                    message = new Message(Encoding.UTF8.GetBytes(msj));
                    Console.WriteLine(msj);
                    await queueClient.SendAsync(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }
        public void GetMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceiveHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            queueClient.RegisterMessageHandler(GetMessageAsync, messageHandlerOptions);
        }
        private static async Task GetMessageAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Number: {message.SystemProperties.SequenceNumber}, Message: {Encoding.UTF8.GetString(message.Body)}");
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
        private static Task ExceptionReceiveHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Error: {exceptionReceivedEventArgs.Exception}");
            return Task.CompletedTask;
        }
        #endregion
    }
}
