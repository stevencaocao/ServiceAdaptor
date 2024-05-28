using MSCore.Util.ConfigurationManager;
using StackExchange.Redis;

namespace MSCore.Util.Redis
{
    public class RedisUtil
    {
        public ConnectionMultiplexer GetRedis()
        {
            string connection = Appsettings.json.GetStringByPath("Redis");
            //using (var redis = ConnectionMultiplexer.Connect(connection))
            //{
            var redis = ConnectionMultiplexer.Connect(connection);
            //database = redis.GetDatabase(db);
            return redis;
            //db.KeyDeleteAsync("a:b");                
            //}

        }
        public static readonly RedisUtil Instance = new RedisUtil();


    }
}
