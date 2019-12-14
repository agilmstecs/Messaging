using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace MessagingCash
{
    class EventHub
    {
        private const string namespaces = "[CONNECTION STRING]";
        private const string name = "messageevents";
        public async Task MessageEvents()
        {
            var connection = new EventHubsConnectionStringBuilder(namespaces)
            {
                EntityPath = name
            };
            EventHubClient client = EventHubClient.CreateFromConnectionString(connection.ToString());
            for (int i = 0; i < 500; i++)
            {
                var message = new
                {
                    content = $"Message: {i}"
                };
                await Console.Out.WriteLineAsync($"Send message: {i}");
                var data = new EventData(
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(message)
                    )
                );
                await client.SendAsync(data);
                await Task.Delay(10);
            }
            await client.CloseAsync();
        }
    }
}
