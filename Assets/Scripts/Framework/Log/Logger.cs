using System.Collections.Generic;

namespace Lite.Framework.Log
{
    public interface ILogger
    {
        void AttachAppender(ILogAppender Appender);
        void DetachAppender(ILogAppender Appender);

        void Write(LogLevel Level, string Format, params object[] Args);
        void Info(string Format, params object[] Args);
        void Warning(string Format, params object[] Args);
        void Error(string Format, params object[] Args);
        void Fatal(string Format, params object[] Args);
    }

    public abstract class LoggerBase : ILogger
    {
        public string Name { get; }
        private readonly List<ILogAppender> AppenderList_;

        protected LoggerBase(string Name)
        {
            this.Name = Name;
            this.AppenderList_ = new List<ILogAppender>();
        }

        public void AttachAppender(ILogAppender Appender)
        {
            AppenderList_.Add(Appender);
        }

        public void DetachAppender(ILogAppender Appender)
        {
            AppenderList_.Remove(Appender);
        }

        public void Write(LogLevel Level, string Format, params object[] Args)
        {
            var Event = new LogEvent(Level, string.Format(Format, Args));
            foreach (var Appender in AppenderList_)
            {
                Appender.Append(Event);
            }
        }

        public void Info(string Format, params object[] Args)
        {
            Write(LogLevel.Info, Format, Args);
        }

        public void Warning(string Format, params object[] Args)
        {
            Write(LogLevel.Warning, Format, Args);
        }

        public void Error(string Format, params object[] Args)
        {
            Write(LogLevel.Error, Format, Args);
        }

        public void Fatal(string Format, params object[] Args)
        {
            Write(LogLevel.Fatal, Format, Args);
        }
    }

    public class Logger : LoggerBase
    {
        private static Logger Default_ = null;

        private static bool Enabled_;
        public static bool Enabled
        {
            get => Enabled_;
            set => EnabledDefaultLogger(value);
        }

        static Logger()
        {
            Enabled_ = false;
            EnabledDefaultLogger(true);
        }

        public Logger(string Name)
            : base(Name)
        {
        }

        private static void EnabledDefaultLogger(bool Value)
        {
            if (Enabled_ == Value)
            {
                return;
            }

            Enabled_ = Value;
            Default_ = GetLogger("Default");
            if (Enabled_)
            {
                Default_.AttachAppender(new LogAppenderConsole(new LogFormatterNormal()));
            }
            else
            {
                Default_.AttachAppender(new LogAppenderEmpty(null));
            }
        }

        public static Logger GetLogger(string Name)
        {
            return new Logger(Name);
        }

        public static void DInfo(string Format, params object[] Args)
        {
            Default_.Info(Format, Args);
        }

        public static void DWarning(string Format, params object[] Args)
        {
            Default_.Warning(Format, Args);
        }

        public static void DError(string Format, params object[] Args)
        {
            Default_.Error(Format, Args);
        }

        public static void DFatal(string Format, params object[] Args)
        {
            Default_.Fatal(Format, Args);
        }
    }
}