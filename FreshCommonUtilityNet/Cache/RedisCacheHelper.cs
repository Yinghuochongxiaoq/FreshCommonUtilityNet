using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Cache
{
    /// <summary>
    /// Redis Link helper
    /// </summary>
    public class RedisCacheHelper
    {
        #region [0、Ready to link redis]
        /// <summary>
        /// load redis connection object.
        /// </summary>
        private static ConnectionMultiplexer Connection => RedisConnectionHelper.GetConnection();

        /// <summary>
        /// Get Current database number.
        /// </summary>
        private static int _currrentDatabaseNumber;

        /// <summary>
        /// Get Current database number.
        /// </summary>
        public static int CurrentDatabaseNumber
        {
            get
            {
                if (_currrentDatabaseNumber > -1) return _currrentDatabaseNumber;
                var db = Connection.GetDatabase();
                if (db != null) return db.Database;
                return -1;
            }
            set { _currrentDatabaseNumber = value; }
        }
        #endregion

        #region [1、Check key exists]

        /// <summary>
        /// Check cache key is exists.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:exists;false :non-existent</returns>
        public static bool Exists(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return cache.KeyExists(key);
        }

        /// <summary>
        /// Check cache key is exists.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:exists;false :non-existent</returns>
        public static async Task<bool> ExistsAsync(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return await cache.KeyExistsAsync(key);
        }
        #endregion

        #region [2、Add config key-value to set.]
        /// <summary>
        /// Add cache into set,the datatype is string.
        /// </summary>
        /// <param name="key">cache key.</param>
        /// <param name="value">cache value</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:add success;false:fail</returns>
        public static bool AddSet(string key, object value, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return cache.StringSet(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

        /// <summary>
        /// Add cache into set,the datatype is string.
        /// </summary>
        /// <param name="key">cache key.</param>
        /// <param name="value">cache value</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:add success;false:fail</returns>
        public static async Task<bool> AddSetAsync(string key, object value, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return await cache.StringSetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

        /// <summary>
        /// Add cache into set,the datatype is string.
        /// </summary>
        /// <param name="key">cache key.</param>
        /// <param name="value">cache value</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <param name="expiressRelative">relative expire timestamp</param>
        /// <returns>true:add success;false:fail</returns>
        public static bool AddSet(string key, object value, TimeSpan expiressRelative, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return cache.StringSet(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiressRelative);
        }

        /// <summary>
        /// Add cache into set,the datatype is string.
        /// </summary>
        /// <param name="key">cache key.</param>
        /// <param name="value">cache value</param>
        /// <param name="expiressRelative">relative expire timestamp</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:add success;false:fail</returns>
        public static async Task<bool> AddSetAsync(string key, object value, TimeSpan expiressRelative, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return await cache.StringSetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiressRelative);
        }

        /// <summary>
        /// Add cache into set,the datatype is string.
        /// </summary>
        /// <param name="key">cache key.</param>
        /// <param name="value">cache value</param>
        /// <param name="expiressAbsoulte">absoulte expire timestamp</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:add success;false:fail</returns>
        public static bool AddSet(string key, object value, DateTime expiressAbsoulte, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var newTime = DateTime.Now;
            TimeSpan? expiressRelative = (expiressAbsoulte - newTime);
            expiressRelative = expiressRelative.Value.TotalMilliseconds <= 0 ? null : expiressRelative;
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return cache.StringSet(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiressRelative);
        }

        /// <summary>
        /// Add cache into set,the datatype is string.
        /// </summary>
        /// <param name="key">cache key.</param>
        /// <param name="value">cache value</param>
        /// <param name="expiressAbsoulte">absoulte expire timestamp</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:add success;false:fail</returns>
        public static async Task<bool> AddSetAsync(string key, object value, DateTime expiressAbsoulte, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var newTime = DateTime.Now;
            TimeSpan? expiressRelative = (expiressAbsoulte - newTime);
            expiressRelative = expiressRelative.Value.TotalMilliseconds <= 0 ? null : expiressRelative;
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return await cache.StringSetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiressRelative);
        }
        #endregion

        #region [3、Add config key-value to list]

        /// <summary>
        /// Add value to list.no-expired
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">object value</param>
        /// <returns>the length of the list after the push operations</returns>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        public static long AddList(string key, object value, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var length = cache.ListLeftPush(key, JsonConvert.SerializeObject(value));
            return length;
        }

        /// <summary>
        /// Add value to list.no-expired
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">object value</param>
        /// <returns>the length of the list after the push operations</returns>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        public static async Task<long> AddListAsync(string key, object value, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var length = await cache.ListLeftPushAsync(key, JsonConvert.SerializeObject(value));
            return length;
        }
        #endregion

        #region [4、Get cache value]

        /// <summary>
        /// Get cache value,just for cache data type set,string.
        /// </summary>
        /// <typeparam name="T">Date type.</typeparam>
        /// <param name="key">cache key.</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns></returns>
        public static T Get<T>(string key, int databaseNumber = -1) where T : class
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var value = cache.StringGet(key);
            if (value != RedisValue.Null && value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// Get cache value,just for cache data type set,string.
        /// </summary>
        /// <typeparam name="T">Date type.</typeparam>
        /// <param name="key">cache key.</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string key, int databaseNumber = -1) where T : class
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var value = await cache.StringGetAsync(key);
            if (value != RedisValue.Null && value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }
        #endregion

        #region [5、Get value from list,just pop stack]

        /// <summary>
        /// Get head last one object from list.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">cache key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>the value of the last element,or nil when key does not exist</returns>
        public static T GetLastOneList<T>(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var value = cache.ListRightPop(key);
            if (value != RedisValue.Null && value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// Get head last one object from list.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">cache key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>the value of the last element,or nil when key does not exist</returns>
        public static async Task<T> GetLastOneListAsync<T>(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var value = await cache.ListRightPopAsync(key);
            if (value != RedisValue.Null && value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">cache key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>the value of the first element, or nil when key does not exist</returns>
        public static T GetFirstOneList<T>(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var value = cache.ListLeftPop(key);
            if (value != RedisValue.Null && value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">cache key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>the value of the first element, or nil when key does not exist</returns>
        public static async Task<T> GetFirstOneListAsync<T>(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var value = await cache.ListLeftPopAsync(key);
            if (value != RedisValue.Null && value.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }
        #endregion

        #region [6、Remove cache key]
        /// <summary>
        /// Remove key
        /// </summary>
        /// <param name="key">config key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns></returns>
        public static bool Remove(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return cache.KeyDelete(key);
        }

        /// <summary>
        /// Async remove key
        /// </summary>
        /// <param name="key">config key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns></returns>
        public static async Task<bool> RemoveAsync(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            return await cache.KeyDeleteAsync(key);
        }
        #endregion

        #region [7、Get Enumerable key collection]

        /// <summary>
        /// Get Enumerable collection
        /// </summary>
        /// <param name="keys">need keys collection</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>value of dictionary.</returns>
        public static IDictionary<string, object> GetKeyDictionary(IEnumerable<string> keys, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            var dict = new Dictionary<string, object>();
            keys.ToList().ForEach(item => dict.Add(item, Get<object>(item, CurrentDatabaseNumber)));
            return dict;
        }
        #endregion

        #region [8、update cache]

        /// <summary>
        /// Update set value.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">value</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:Update Success;false:Update fail</returns>
        public static bool ReplaceSet(string key, object value, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            if (cache.KeyExists(key)) if (!cache.KeyDelete(key)) return false;
            return cache.StringSet(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Update set value.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">value</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:Update Success;false:Update fail</returns>
        public static async Task<bool> ReplaceSetAsync(string key, object value, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var isExists = await cache.KeyExistsAsync(key);
            if (isExists)
            {
                var isDelete = await cache.KeyDeleteAsync(key);
                if (!isDelete) return false;
            }
            return await cache.StringSetAsync(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Update set value.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">value</param>
        /// <param name="expiressRelative">relative expire timestamp</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:Update Success;false:Update fail</returns>
        public static bool ReplaceSet(string key, object value, TimeSpan expiressRelative, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            if (cache.KeyExists(key)) if (!cache.KeyDelete(key)) return false;
            return cache.StringSet(key, JsonConvert.SerializeObject(value), expiressRelative);
        }

        /// <summary>
        /// Update set value.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">value</param>
        /// <param name="expiressRelative">relative expire timestamp</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:Update Success;false:Update fail</returns>
        public static async Task<bool> ReplaceSetAsync(string key, object value, TimeSpan expiressRelative, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var isExists = await cache.KeyExistsAsync(key);
            if (isExists)
            {
                var isDelete = await cache.KeyDeleteAsync(key);
                if (!isDelete) return false;
            }
            return await cache.StringSetAsync(key, JsonConvert.SerializeObject(value), expiressRelative);
        }

        /// <summary>
        /// Update set value.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">value</param>
        /// <param name="expiressAbsoulte">absoulte expire timestamp</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:Update Success;false:Update fail</returns>
        public static bool ReplaceSet(string key, object value, DateTime expiressAbsoulte, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            if (cache.KeyExists(key)) if (!cache.KeyDelete(key)) return false;
            var newTime = DateTime.Now;
            TimeSpan? expiressRelative = (expiressAbsoulte - newTime);
            expiressRelative = expiressRelative.Value.TotalMilliseconds <= 0 ? null : expiressRelative;
            return cache.StringSet(key, JsonConvert.SerializeObject(value), expiressRelative);
        }

        /// <summary>
        /// Update set value.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">value</param>
        /// <param name="expiressAbsoulte">absoulte expire timestamp</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>true:Update Success;false:Update fail</returns>
        public static async Task<bool> ReplaceSetAsync(string key, object value, DateTime expiressAbsoulte, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var isExists = await cache.KeyExistsAsync(key);
            if (isExists)
            {
                var isDelete = await cache.KeyDeleteAsync(key);
                if (!isDelete) return false;
            }
            var newTime = DateTime.Now;
            TimeSpan? expiressRelative = (expiressAbsoulte - newTime);
            expiressRelative = expiressRelative.Value.TotalMilliseconds <= 0 ? null : expiressRelative;
            return await cache.StringSetAsync(key, JsonConvert.SerializeObject(value), expiressRelative);
        }
        #endregion

        #region [9、Get list count]

        /// <summary>
        /// Update set value.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>Returns the length of the list stored at key. If key does not exist, it is interpreted as an empty list and 0 is returned.the length of the list at key.</returns>
        public static long GetListLength(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var length = cache.ListLength(key);
            return length;
        }

        /// <summary>
        /// Update set value.
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>Returns the length of the list stored at key. If key does not exist, it is interpreted as an empty list and 0 is returned.the length of the list at key.</returns>
        public static async Task<long> GetListLengthAsync(string key, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var length = await cache.ListLengthAsync(key);
            return length;
        }
        #endregion

        #region [10、Remove list key-value]
        /// <summary>
        /// Remove value to list.no-expired
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">object value</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>the number of removed elements.</returns>
        public static long RemoveValueList(string key, object value, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var length = cache.ListRemove(key, JsonConvert.SerializeObject(value));
            return length;
        }

        /// <summary>
        /// Remove value to list.no-expired
        /// </summary>
        /// <param name="key">cache key</param>
        /// <param name="value">object value</param>
        /// <param name="databaseNumber">data base number,if -1 will get default database number,others need you base number range.</param>
        /// <returns>the number of removed elements.</returns>
        public static async Task<long> RemoveValueListAsync(string key, object value, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var length = await cache.ListRemoveAsync(key, JsonConvert.SerializeObject(value));
            return length;
        }
        #endregion

        #region [11、Get list range info]

        /// <summary>
        /// Get list range info.
        /// </summary>
        /// <typeparam name="T">type T</typeparam>
        /// <param name="key">cache key</param>
        /// <param name="start">start index,begin is zore.</param>
        /// <param name="fail">the fail of list.</param>
        /// <param name="databaseNumber">database number.</param>
        /// <returns></returns>
        public static List<T> GetListRange<T>(string key, long start = 0, long fail = -1, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var valueArray = cache.ListRange(key, start, fail);
            var resulteList = new List<T>();
            if (valueArray != null && valueArray.Any())
            {
                resulteList = valueArray.Select(f => JsonConvert.DeserializeObject<T>(f)).ToList();
            }
            return resulteList;
        }

        /// <summary>
        /// Get list range info.
        /// </summary>
        /// <typeparam name="T">type T</typeparam>
        /// <param name="key">cache key</param>
        /// <param name="start">start index,begin is zore.</param>
        /// <param name="fail">the fail of list.</param>
        /// <param name="databaseNumber">database number.</param>
        /// <returns></returns>
        public static async Task<List<T>> GetListRangeAsync<T>(string key, long start = 0, long fail = -1, int databaseNumber = -1)
        {
            CurrentDatabaseNumber = databaseNumber;
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            var cache = Connection.GetDatabase(CurrentDatabaseNumber);
            var valueArray = await cache.ListRangeAsync(key, start, fail);
            var resulteList = new List<T>();
            if (valueArray != null && valueArray.Any())
            {
                resulteList =
                    valueArray.Select(f => Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(f)).Result)
                        .ToList();
            }
            return resulteList;
        }
        #endregion
    }
}