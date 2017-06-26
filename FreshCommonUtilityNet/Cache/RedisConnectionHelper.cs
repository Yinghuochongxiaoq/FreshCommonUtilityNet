using System;
using FreshCommonUtility.Configure;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Cache
{
    /// <summary>
    /// FreshMan redis connection helper.Config redis link string key is :RedisConnectionString
    /// </summary>
    public class RedisConnectionHelper
    {
        /// <summary>
        /// Return connection object.
        /// </summary>
        private static ConnectionMultiplexer _conn;

        /// <summary>
        /// Lock resource
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// cut put point.
        /// </summary>
        private RedisConnectionHelper() { }

        /// <summary>
        /// Get single connection object
        /// </summary>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnection()
        {
            if (_conn == null)
            {
                lock (SyncRoot)
                {
                    if (_conn == null)
                    {
                        var connectionString = AppConfigurationHelper.GetString("RedisConnectionString");
                        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Redis connection config is Empty,please check you config.");
                        _conn = ConnectionMultiplexer.Connect(connectionString);
                    }
                }
            }
            return _conn;
        }
    }
}
