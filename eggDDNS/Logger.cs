using System.Text;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;
namespace eggDDNS
{   
    class Logger
    {   static Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle> customThemeStyles = new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
        {
            {
                ConsoleThemeStyle.Text, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.White,  // Default text color
                }
            },
            {
                ConsoleThemeStyle.String, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.Cyan,  // Color for string literals
                }
            },
            {
                ConsoleThemeStyle.Number, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.Magenta,  // Color for string literals
                }
            },
            {
                ConsoleThemeStyle.Boolean, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.Yellow,  // Color for string literals
                }
            },
            {
                ConsoleThemeStyle.Scalar, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.Green,  // Color for string literals
                }
            },
            {
                ConsoleThemeStyle.Name, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.DarkBlue,  // Color for string literals
                }
            },
            {
                ConsoleThemeStyle.SecondaryText, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.DarkGray,  // Color for string literals
                }
            },
            {
                ConsoleThemeStyle.TertiaryText, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.DarkMagenta,  // Color for string literals
                }
            },
            {
                ConsoleThemeStyle.LevelInformation, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.Blue,  // Color for Information level
                }
            },
            {
                ConsoleThemeStyle.LevelWarning, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.Yellow,  // Color for Warning level
                }
            },
            {
                ConsoleThemeStyle.LevelError, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.Red,  // Color for Error level
                }
            },
            {
                ConsoleThemeStyle.LevelDebug, new SystemConsoleThemeStyle
                {
                    Foreground = ConsoleColor.Gray,  // Color for Debug level
                }
            },
        };
        class LogFileHook: FileLifecycleHooks
        {
            public override Stream OnFileOpened(string path, Stream underlyingStream, Encoding encoding)
            {   
                 lastWritedFilename =path;              
                return base.OnFileOpened(underlyingStream, encoding);
            }
        }

      public  static string?   lastWritedFilename = null;

        public static void Init()
        {
      
            var customTheme = new SystemConsoleTheme(customThemeStyles);
    
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(GetLogFilePath(), rollingInterval: RollingInterval.Day,restrictedToMinimumLevel:LogEventLevel.Verbose)
                .WriteTo.Console(theme: customTheme)
                .CreateLogger();
                
            Log.Logger = loggerConfig;

            var hook = new LogFileHook();
            var OnlyToGetFilePath = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(GetLogFilePath(), rollingInterval: RollingInterval.Day, hooks:hook,restrictedToMinimumLevel:LogEventLevel.Verbose)
                .CreateLogger();

            OnlyToGetFilePath.Write(LogEventLevel.Verbose, "Logger has been created."); // Mensaje vacío
    
        }

        private static string GetLogOsPath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return "/var/log"; // Linux or macOS
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // Windows or other
            }
        }
        private static string GetLogFilePath()
        {
            string osSpecificDirectory = GetLogOsPath();
            string logDirectory = Path.Combine(osSpecificDirectory, Constants.ApplicationName);
            Directory.CreateDirectory(logDirectory);
            return Path.Combine(logDirectory, Constants.ApplicationName + ".log");
        }
        public static void Info(string message, params object?[] propertyValues)
        {
            Log.Information(message, propertyValues);
        }

        public static void Warning(string message, params object?[] propertyValues)
        {
            Log.Warning(message, propertyValues);
        }

        public static void Debug(string message, params object?[] propertyValues)
        {
            Log.Debug(message, propertyValues);
        }

        public static void Critical(string message, params object?[] propertyValues)
        {
            Log.Error(message, propertyValues);
        }
    }
}