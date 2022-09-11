using UnityEngine;

namespace WaveTools {
    public enum LoggingLevel { Normal, Warning, Error, None }

    public static class Logging {

        public static void Log(string message, LoggingLevel loggingLevel = LoggingLevel.Normal) {
            if(loggingLevel < Generator.instance.loggingLevel) return;
            switch(loggingLevel) {
                case LoggingLevel.Normal:
                    Debug.Log(message);
                    break;
                case LoggingLevel.Warning:
                    Debug.LogWarning(message);
                    break;
                case LoggingLevel.Error:
                    Debug.LogError(message);
                    break;
                default:
                    break;
            }
        }
    }
}
