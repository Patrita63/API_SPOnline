using Serilog;
using System;
using System.Text;

namespace ClassLibraryLogging
{
    // Serilog: read AppSettings
    // https://github.com/serilog/serilog/wiki/Configuration-Basics
    // [{Level:u3}]   3 si riferisce alle lettere dentro [INF]
    //2020 - 04 - 29 12:40:57.108[INF] information
    //2020 - 04 - 29 12:40:57.108[WRN] warning
    //2020 - 04 - 29 12:40:57.069[ERR] error
    //2020 - 04 - 29 12:40:57.107[FTL] fatal

    // Level Usage

    // 1 Verbose       Verbose is the noisiest level, rarely(if ever) enabled for a production app.
    // 2 Debug         Debug is used for internal system events that are not necessarily observable from the outside, but useful when determining how something happened.
    // 3 Information   Information events describe things happening in the system that correspond to its responsibilities and functions.Generally these are the observable actions the system can perform.
    // 4 Warning       When service is degraded, endangered, or may be behaving outside of its expected parameters, Warning level events are used.
    // 5 Error         When functionality is unavailable or expectations broken, an Error event is used.
    // 6 Fatal         The most critical level, Fatal events demand immediate attention.

    // Default Level - if no MinimumLevel is specified, then Information level events and higher will be processed.
    // WEB.CONFIG
    //< add key = "serilog:minimum-level" value = "Verbose" />

    // GLOBAL.ASAX.CS
    // Log.Logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();
    //Log.Logger.Verbose("verbose");
    //Log.Logger.Debug("debug");
    //Log.Logger.Information("information");
    //Log.Logger.Warning("warning");
    //Log.Logger.Error("error");
    //Log.Logger.Fatal("fatal");

    static public class SeriLogging
    {
        /// <summary>
        /// Verbose is the noisiest level, rarely(if ever) enabled for a production app.
        /// </summary>
        /// <param name="UtenteLoggato"></param>
        /// <param name="Message"></param>
        public static void LogVerbose(string UtenteLoggato, string Message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}] - {1}", UtenteLoggato, Message));
            Log.Logger.Verbose(sb.ToString());
        }

        /// <summary>
        /// Debug is used for internal system events that are not necessarily observable from the outside, but useful when determining how something happened.
        /// </summary>
        /// <param name="UtenteLoggato"></param>
        /// <param name="Message"></param>
        public static void LogDebug(string UtenteLoggato, string Message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}] - {1}", UtenteLoggato, Message));
            Log.Logger.Debug(sb.ToString());
        }

        /// <summary>
        /// Information events describe things happening in the system that correspond to its responsibilities and functions.Generally these are the observable actions the system can perform.
        /// </summary>
        /// <param name="UtenteLoggato"></param>
        /// <param name="Message"></param>
        public static void LogInformation(string UtenteLoggato, string Message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}] - {1}", UtenteLoggato, Message));
            Log.Logger.Information(sb.ToString());
        }

        /// <summary>
        /// When service is degraded, endangered, or may be behaving outside of its expected parameters, Warning level events are used.
        /// </summary>
        /// <param name="UtenteLoggato"></param>
        /// <param name="Message"></param>
        public static void LogWarning(string UtenteLoggato, string Message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}] - {1}", UtenteLoggato, Message));
            Log.Logger.Warning(sb.ToString());
        }

        /// <summary>
        /// When functionality is unavailable or expectations broken, an Error event is used.
        /// </summary>
        /// <param name="UtenteLoggato"></param>
        /// <param name="Message"></param>
        public static void LogError(string UtenteLoggato, string Message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}] - {1}", UtenteLoggato, Message));
            Log.Logger.Error(sb.ToString());
        }

        /// <summary>
        /// The most critical level, Fatal events demand immediate attention.
        /// </summary>
        /// <param name="UtenteLoggato"></param>
        /// <param name="Message"></param>
        public static void LogFatal(string UtenteLoggato, string Message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}] - {1}", UtenteLoggato, Message));
            Log.Logger.Fatal(sb.ToString());
        }

    }
}