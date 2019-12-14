using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace MessagingCash
{
    class Redis
    {
        const string connString = "[CONNECTION STRING]";
        public void AddToRedis(string name, string value)
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(connString);
            });
            IDatabase cache = lazyConnection.Value.GetDatabase();
            cache.StringSet(name, value);
            lazyConnection.Value.Dispose();
        }
        public void ReadFromRedis(string name)
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(connString);
            });
            IDatabase cache = lazyConnection.Value.GetDatabase();
            Console.WriteLine("Value: {0}",cache.StringGet(name).ToString());
            lazyConnection.Value.Dispose();
        }
    }
}