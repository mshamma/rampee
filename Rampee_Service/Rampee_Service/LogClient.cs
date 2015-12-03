using NLog;

namespace Rampee_Service
{
    class LogClient
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Record(int e, string message)
        {
            var eString = e.ToString();
            logger.Log(LogLevel.Info, string.Format("{0}: {1}", eString, message));
        }
    }
}