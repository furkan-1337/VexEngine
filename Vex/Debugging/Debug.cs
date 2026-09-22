using System;
using System.Collections.Generic;
using System.Text;

namespace Vex.Debugging
{
    public class Debug
    {
        public static bool IsEnabled { get; set; } = true;

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (!IsEnabled) return;
            Console.ResetColor();
            Console.Write("[");
            switch (level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Engine:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
            }
            Console.Write($"{level}");
            Console.ResetColor();
            Console.Write("]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($" {message}");
            Console.ResetColor();
        }

        public static void Log(LogLevel level, string message)
        {
            Log(message, level);
        }

        public enum LogLevel
        {
            Engine,
            Info,
            Warning,
            Error
        }
    }
}
