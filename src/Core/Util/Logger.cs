using System;
using System.IO;

namespace CnSharp.Updater.Util
{
    public class Logger
    {
        /// <summary>
        /// 多线程锁
        /// </summary>
        private readonly object _locker = new object();

        private readonly string _logFolder;

        public Logger(string logFolder)
        {
            this._logFolder = logFolder;
        }

        public string LogFolder
        {
            get { return _logFolder; }
        }


        private void Write(string fileName, string type, string msg)
        {
            EnsureLogFolder();
            string file = Path.Combine(_logFolder, fileName);
            lock (_locker)
            {
                using (var sw = new StreamWriter(file, true))
                {
                    sw.WriteLine("{0} - [{2}]: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg, type);
                }
            }
        }

        /// <summary>
        /// 写调试日志
        /// </summary>
        /// <param name="msg">日志消息</param>
        public void WriteDebugLog(string msg)
        {
            Write("Debug.log", "DEBUG", msg);
        }

        /// <summary>
        /// 写异常日志
        /// </summary>
        /// <param name="msg">日志消息</param>
        public void WriteExceptionLog(string msg)
        {
            Write("Exception.log", "Exception", msg);
        }

        /// <summary>
        /// 写异常日志
        /// </summary>
        /// <param name="exception">异常</param>
        public void WriteExceptionLog(Exception exception)
        {
            Write("Exception.log", "Exception", exception.Message + Environment.NewLine + exception.StackTrace);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">日志消息</param>
        public void WriteLog(string msg)
        {
            Write("Log.log", "Log", msg);
        }

        /// <summary>
        /// 创建日志存储文件夹
        /// </summary>
        private void EnsureLogFolder()
        {
            if (Directory.Exists(_logFolder) != true)
                Directory.CreateDirectory(_logFolder);
        }
    }
}