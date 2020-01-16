using System;

namespace MessagingCash
{
    class Program
    {
        static void Main(string[] args)
        {
            //Singleton instance = new Singleton();
            //instance.CreateVm().Wait();
            
            //UseRedis();
            //UseSearch();
            //UseEventHub();
            //UseSBqueue();
            //UseSAqueue();            
        }

        static private void UseRedis()
        {
            Console.WriteLine("Welcome, What do you want to do? \n a) Add values to the redis cache \n r) Read a value from redis cache \n e) Exit");
            string e = Console.ReadLine();
            Redis redis = new Redis();
            while (e != "e")
            {
                string name, value;
                switch (e)
                {
                    case "a":
                        Console.WriteLine("Name:");
                        name = Console.ReadLine();
                        Console.WriteLine("Value:");
                        value = Console.ReadLine();
                        redis.AddToRedis(name, value);
                        break;
                    case "r":
                        Console.WriteLine("Name:");
                        name = Console.ReadLine();
                        redis.ReadFromRedis(name);
                        break;
                }
                System.Threading.Thread.Sleep(100);
                Console.WriteLine("\n a) Add values to the redis cache \n r) Read a value from redis cache \n e) Exit");
                e = Console.ReadLine();
            }
        }
        static private void UseSearch()
        {
            Search search = new Search();
            //search.CreateIndex();
            //search.CreateDocument();
            search.SearchingQueries();
        }
        static private void UseEventHub()
        {
            EventHub eh = new EventHub();
            eh.MessageEvents().Wait();
        }
        static private void UseSBqueue()
        {
            MessagingSB messaging = new MessagingSB();

            Console.WriteLine("Welcome, What do you want to do? \n a) Add messages \n g) Get messages \n e) Exit");
            string e = Console.ReadLine();
            while (e != "e")
            {
                switch (e)
                {
                    case "a":
                        Console.WriteLine("Total of messages to add:");
                        string totalMessages = Console.ReadLine();
                        messaging.SetMessagesAsync(Convert.ToInt32(totalMessages)).Wait();
                        break;
                    case "g":
                        messaging.GetMessages();
                        break;
                }
                System.Threading.Thread.Sleep(100);
                Console.WriteLine("\n a) Add messages \n g) Get messages \n e) Exit");
                e = Console.ReadLine();
            }
        }
        static private void UseSAqueue()
        {
            MessagingSA messaging = new MessagingSA();
            Console.WriteLine("Welcome, What do you want to do? \n a) Add messages \n p) Peek next message \n g) Get next message \n u) Update next message \n e) Exit");
            string e = Console.ReadLine();
            while (e != "e")
            {
                string total, value;
                switch (e)
                {
                    case "a":
                        Console.WriteLine("Total of messages to add:");
                        total = Console.ReadLine();
                        messaging.AddQueueMessage(Convert.ToInt32(total)).Wait();
                        break;
                    case "p":
                        messaging.PeekQueueMessage().Wait();
                        break;
                    case "g":
                        messaging.GetQueueMessage().Wait();
                        break;
                    case "u":
                        Console.WriteLine("New message:");
                        value = Console.ReadLine();
                        messaging.UpdateQueueMessage(value).Wait();
                        break;
                }
                System.Threading.Thread.Sleep(100);
                Console.WriteLine("\n a) Add messages \n p) Peek next message \n g) Get next message \n u) Update next message \n e) Exit");
                e = Console.ReadLine();
            }
        }
    }
}
