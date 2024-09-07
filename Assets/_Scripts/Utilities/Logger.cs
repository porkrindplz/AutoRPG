using System.Runtime.CompilerServices;
using UnityEngine;

namespace _Scripts.Utilities
{
    public static class Logger
    {
        // usage : Logger.Log("Something Happened");
        //            : Logger.Log($" Log some variable { i }");
        //
        public static void Log(string msg, [CallerMemberName] string methodName = null, [CallerFilePath] string fileName = null, [CallerLineNumber] int lineNo = -1)
        {
        
            string shortFileName = System.IO.Path.GetFileName(fileName);
        
            msg= $"{shortFileName}({lineNo}): {msg??"NULL"} ({methodName})";
            UnityEngine.Debug.LogFormat( LogType.Log, LogOption.NoStacktrace, null, "{0}", msg ?? "NULL" );
        }

    }
}