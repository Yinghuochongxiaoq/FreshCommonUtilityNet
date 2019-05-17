using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshCommonUtilityNet.Log
{
    /// <summary>
    /// 写日志类
    /// </summary>
    public class LogUtil
    {
        #region 字段
        private static object _lock = new object();
        private static int _fileSize = 10 * 1024 * 1024; //日志分隔文件大小
        private static string _strNow = DateTime.Now.ToString("yyyyMMdd");

        private static string _pathInfoLog = "Log\\Info";
        private static string _pathErrorLog = "Log\\Error";
        private static int _indexInfoLog = 1;
        private static int _indexErroLog = 1;
        private static long _sizeInfoLog = 0;
        private static long _sizeErrorLog = 0;
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path">日志路径</param>
        /// <param name="fileSize">日志分隔文件大小</param>
        public static void Init(string path, int? fileSize = null)
        {
            _pathInfoLog = Path.Combine(path, "Log\\Info");
            _pathErrorLog = Path.Combine(path, "Log\\Error");
            var root = AppDomain.CurrentDomain.BaseDirectory;
            _pathInfoLog = root + _pathInfoLog;
            _pathErrorLog = root + _pathErrorLog;
            if (fileSize != null) _fileSize = fileSize.Value;
            if (!Directory.Exists(_pathInfoLog)) Directory.CreateDirectory(_pathInfoLog);
            if (!Directory.Exists(_pathErrorLog)) Directory.CreateDirectory(_pathErrorLog);

            InitIndex(_pathInfoLog, ref _indexInfoLog, ref _sizeInfoLog);
            InitIndex(_pathErrorLog, ref _indexErroLog, ref _sizeErrorLog);
        }
        #endregion

        #region 初始化文件index和size
        private static void InitIndex(string path, ref int index, ref long fileSize)
        {
            List<string> fileList = Directory.GetFiles(path, _strNow + "*").ToList();
            fileSize = 0;
            index = 1;
            string logPath = CreateLogPath(path, index);
            while (fileList.Exists(a => a == logPath))
            {
                FileInfo fileInfo = new FileInfo(logPath);
                if (fileInfo.Length < _fileSize)
                {
                    fileSize = fileInfo.Length;
                    break;
                }
                index++;
                logPath = CreateLogPath(path, index);
            }
        }
        #endregion

        #region 写文件
        /// <summary>
        /// 写文件
        /// </summary>
        private static void WriteFile(string log, string path, ref long fileSize)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(log);
                        sw.Flush();
                        fileSize = fs.Length;
                    }
                    fs.Close();
                }
            }
            catch { }
        }
        #endregion

        #region 生成日志文件路径
        /// <summary>
        /// 生成日志文件路径
        /// </summary>
        private static string CreateLogPath(string path, int index)
        {
            if (_pathInfoLog == null) throw new Exception("请先执行LogUtil.Init方法初始化");

            return Path.Combine(path, _strNow + (index == 1 ? string.Empty : "_" + index) + ".txt");
        }

        /// <summary>
        /// 生成日志文件路径
        /// </summary>
        private static string CreateLogPath(string path, ref int index, bool isInfoLog)
        {
            if ((isInfoLog && _sizeInfoLog > _fileSize)
                || (!isInfoLog && _sizeErrorLog > _fileSize))
            {
                index++;
            }

            return CreateLogPath(path, index);
        }
        #endregion

        #region 写错误日志
        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void LogError(string log)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    WriteFile(string.Format(@"{0} [Error]   {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), log), CreateLogPath(_pathErrorLog, ref _indexErroLog, false), ref _sizeErrorLog);
                }
            });
        }
        #endregion

        #region 写操作日志
        /// <summary>
        /// 写操作日志
        /// </summary>
        public static void Log(string log)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    WriteFile(string.Format(@"{0} [Info]   {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), log), CreateLogPath(_pathInfoLog, ref _indexInfoLog, true), ref _sizeInfoLog);
                }
            });
        }

        /// <summary>
        /// 写自定义内容
        /// </summary>
        /// <param name="log"></param>
        public static void LogContent(string log)
        {
            Task.Factory.StartNew(() =>
            {
                lock (_lock)
                {
                    WriteFile(log, CreateLogPath(_pathInfoLog, ref _indexInfoLog, true), ref _sizeInfoLog);
                }
            });
        }
        #endregion

    }
}
