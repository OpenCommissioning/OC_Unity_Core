using UnityEngine;
using Object = UnityEngine.Object;

namespace OC.Logging
{
    public static class Logger 
    {
        public const string VERBOSE_TAG = "<color=yellow>Verbose</color>";

        public static void Log(object message)
        {
            Debug.unityLogger.Log(LogType.Log, Package.NAME_RAW, message);
        }
        
        public static void Log(object message, Object context)
        {
            Debug.unityLogger.Log(LogType.Log, Package.NAME_RAW, message, context);
        }
        
        public static void LogWarning(object message)
        {
            Debug.unityLogger.Log(LogType.Warning, Package.NAME_RAW, message);
        }
        
        public static void LogWarning(object message, Object context)
        {
            Debug.unityLogger.Log(LogType.Warning, Package.NAME_RAW, message, context);
        }
        
        public static void LogError(object message)
        {
            Debug.unityLogger.Log(LogType.Error, Package.NAME_RAW, message);
        }
        
        public static void LogError(object message, Object context)
        {
            Debug.unityLogger.Log(LogType.Error, Package.NAME_RAW, message, context);
        }
        
        public static void Log(LogType logType, object message)
        {
            Debug.unityLogger.Log(logType, Package.NAME_RAW, message);
        }
        
        public static void LogVerbose(LogType logType, object message)
        {
            Debug.unityLogger.Log(logType, $"{Package.NAME_RAW} {VERBOSE_TAG}", message);
        }

        public static void Log(LogType logType, object message, Object context)
        {
            Debug.unityLogger.Log(logType, Package.NAME_RAW, message, context);
        }
        
        public static void LogVerbose(LogType logType, object message, Object context)
        {
            Debug.unityLogger.Log(logType, $"{Package.NAME_RAW} {VERBOSE_TAG}", message, context);
        }

        public static void LogFormat(LogType logType, string format, params object[] args)
        {
            Debug.unityLogger.Log(logType, Package.NAME_RAW, string.Format(format, args));
        }
        
        public static void LogFormat(LogType logType,Object context, string format, params object[] args)
        {
            Debug.unityLogger.Log(logType, Package.NAME_RAW, string.Format(format, args), context);
        }
    }
}