﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LiteFramework.Core.Log
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

        public void Write(LogLevel Level, string Msg)
        {
            var Event = new LogEvent(Level, Msg);
            foreach (var Appender in AppenderList_)
            {
                Appender.Append(Event);
            }
        }

        public void Write(LogLevel Level, string Format, params object[] Args)
        {
            Write(Level, string.Format(Format, Args));
        }

        public void Info(string Msg)
        {
            Write(LogLevel.Info, Msg);
        }

        public void Info(string Format, params object[] Args)
        {
            Write(LogLevel.Info, Format, Args);
        }

        public void Warning(string Msg)
        {
            Write(LogLevel.Warning, Msg);
        }

        public void Warning(string Format, params object[] Args)
        {
            Write(LogLevel.Warning, Format, Args);
        }

        public void Error(string Msg)
        {
            Write(LogLevel.Error, Msg);
        }

        public void Error(string Format, params object[] Args)
        {
            Write(LogLevel.Error, Format, Args);
        }

        public void Fatal(string Msg)
        {
            Write(LogLevel.Fatal, Msg);
        }

        public void Fatal(string Format, params object[] Args)
        {
            Write(LogLevel.Fatal, Format, Args);
        }
    }

    public class LLogger : LoggerBase
    {
        private static LLogger Default_ = null;

        private static bool Enabled_;
        public static bool Enabled
        {
            get => Enabled_;
            set => EnabledDefaultLogger(value);
        }

        static LLogger()
        {
            Enabled_ = false;
            EnabledDefaultLogger(true);
        }

        public LLogger(string Name)
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
#if UNITY_EDITOR
                Default_.AttachAppender(new LogAppenderUnityEditor(new LogFormatterNormal()));
#endif
                Debug.unityLogger.logEnabled = true;
            }
            else
            {
                Default_.AttachAppender(new LogAppenderEmpty(null));
                Debug.unityLogger.logEnabled = false;
            }
        }

        public static LLogger GetLogger(string Name)
        {
            return new LLogger(Name);
        }

        public static void LInfo(string Msg)
        {
            Default_.Info(Msg);
        }

        public static void LInfo(string Format, params object[] Args)
        {
            Default_.Info(Format, Args);
        }

        public static void LWarning(string Msg)
        {
            Default_.Warning(Msg);
        }

        public static void LWarning(string Format, params object[] Args)
        {
            Default_.Warning(Format, Args);
        }

        public static void LError(string Msg)
        {
            Default_.Error(Msg);
        }

        public static void LError(string Format, params object[] Args)
        {
            Default_.Error(Format, Args);
        }

        public static void LFatal(string Msg)
        {
            Default_.Fatal(Msg);
        }

        public static void LFatal(string Format, params object[] Args)
        {
            Default_.Fatal(Format, Args);
        }


#if UNITY_EDITOR
        private static string GetStackTrace()
        {
            var ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var FieldInfo = ConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            var ConsoleWindowInstance = FieldInfo.GetValue(null);
            if (ConsoleWindowInstance != null)
            {
                if ((object)UnityEditor.EditorWindow.focusedWindow == ConsoleWindowInstance)
                {
                    FieldInfo = ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    var ActiveText = FieldInfo.GetValue(ConsoleWindowInstance).ToString();
                    return ActiveText;
                }
            }

            return null;
        }

        [UnityEditor.Callbacks.OnOpenAsset(0)]
        public static bool OnOpenAsset(int InstanceID, int Line)
        {
            if (Line == -1)
            {
                return false;
            }

            var StackTrace = GetStackTrace();
            if (!string.IsNullOrEmpty(StackTrace) && StackTrace.Contains("/Log/"))
            {
                var Matches = Regex.Match(StackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);

                while (Matches.Success)
                {
                    var PathLine = Matches.Groups[1].Value;
                    if (!PathLine.Contains("/Log/"))
                    {
                        var SplitIndex = PathLine.LastIndexOf(":");
                        if (!int.TryParse(PathLine.Substring(SplitIndex + 1), out Line))
                        {
                            return false;
                        }
                        var Path = PathLine.Substring(0, SplitIndex);
                        var FullPath = $"{Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"))}{Path}";
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(FullPath.Replace('/', '\\'), Line);
                        return true;
                    }

                    Matches = Matches.NextMatch();
                }
            }

            return false;
        }
#endif
    }
}