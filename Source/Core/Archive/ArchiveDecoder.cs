﻿using System;
using System.Collections.Generic;
using LiteFramework.Core.Log;

namespace LiteFramework.Core.Archive
{
    public class ArchiveDecoder
    {
        private readonly ArchiveReadStream Stream_;
        private readonly Dictionary<string, object> CacheList_;

        public ArchiveDecoder(ArchiveReadStream Stream)
        {
            Stream_ = Stream;
            CacheList_ = new Dictionary<string, object>();
        }

        /// <summary>
        /// support type see : ArchiveDataType
        /// </summary>
        public T Read<T>(string Key, T Default)
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return Default;
            }

            if (CacheList_[Key] is T TValue)
            {
                return TValue;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return Default;
        }

        /// <summary>
        /// only support (Boolean, Int32, BigInteger, String)
        /// </summary>
        public T[] ReadArray<T>(string Key, T[] Default)
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return Default;
            }

            if (CacheList_[Key] is T[] Value)
            {
                if (Default != null && Value.Length < Default.Length)
                {
                    var NewValue = new T[Default.Length];

                    for (var Index = 0; Index < NewValue.Length; ++Index)
                    {
                        if (Index < Value.Length)
                        {
                            NewValue[Index] = Value[Index];
                        }
                        else
                        {
                            NewValue[Index] = Default[Index];
                        }
                    }

                    return NewValue;
                }

                return Value;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return Default;
        }

        /// <summary>
        /// only support (Int32, BigInteger)
        /// </summary>
        public T[,] ReadArray2<T>(string Key, T[,] Default)
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return Default;
            }

            if (CacheList_[Key] is T[,] Value)
            {
                var Len1 = Value.GetLength(0);
                var Len2 = Value.GetLength(1);
                var NewLen1 = Default?.GetLength(0) ?? Len1;
                var NewLen2 = Default?.GetLength(1) ?? Len2;

                if (Len1 < NewLen1 || Len2 < NewLen2)
                {
                    var NewValue = new T[NewLen1, NewLen2];

                    for (var X = 0; X < NewLen1; ++X)
                    {
                        for (var Y = 0; Y < NewLen2; ++Y)
                        {
                            if (X < Len1 && Y < Len2)
                            {
                                NewValue[X, Y] = Value[X, Y];
                            }
                            else
                            {
                                NewValue[X, Y] = Default[X, Y];
                            }
                        }
                    }

                    return NewValue;
                }

                return Value;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return Default;
        }

        public Dictionary<TK, TV> ReadDictionary<TK, TV>(string Key, Dictionary<TK, TV> Def)
        {
            var KeyList = ReadArray<TK>(Key + "_key", null);
            var ValueList = ReadArray<TV>(Key + "_value", null);
            if (KeyList == null || ValueList == null || KeyList.Length != ValueList.Length)
            {
                return Def;
            }

            var OutputDictionary = new Dictionary<TK, TV>();
            for (var Index = 0; Index < KeyList.Length; ++Index)
            {
                OutputDictionary[KeyList[Index]] = ValueList[Index];
            }

            return OutputDictionary;
        }

        public T ReadSub<T>(string Key) where T : IArchiveInfo, new()
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return new T();
            }

            if (CacheList_[Key] is T Info)
            {
                return Info;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return new T();
        }

        public T[] ReadSubArray<T>(string Key, T[] Default) where T : IArchiveInfo, new()
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return Default;
            }

            if (CacheList_[Key] is object[] ArrayObj)
            {
                var Len = Default?.Length ?? ArrayObj.Length;
                var OutputArray = new T[Len];
                for (var Index = 0; Index < Len; ++Index)
                {
                    if (Index < ArrayObj.Length)
                    {
                        OutputArray[Index] = (T) ArrayObj[Index];
                    }
                    else
                    {
                        OutputArray[Index] = Default[Index];
                    }
                }

                return OutputArray;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return Default;
        }

        public T[,] ReadSubArray2<T>(string Key, T[,] Default) where T : IArchiveInfo, new()
        {
            if (!CacheList_.ContainsKey(Key))
            {
                return Default;
            }

            if (CacheList_[Key] is object[,] ArrayObj)
            {
                var Len1 = ArrayObj.GetLength(0);
                var Len2 = ArrayObj.GetLength(1);
                var Width = Default?.GetLength(0) ?? Len1;
                var Height = Default?.GetLength(1) ?? Len2;
                var OutputArray = new T[Width, Height];

                for (var X = 0; X < Width; ++X)
                {
                    for (var Y = 0; Y < Height; ++Y)
                    {
                        if (X < Len1 && Y < Len2)
                        {
                            OutputArray[X, Y] = (T) ArrayObj[X, Y];
                        }
                        else
                        {
                            OutputArray[X, Y] = Default[X, Y];
                        }
                    }
                }

                return OutputArray;
            }

            LLogger.LWarning($"archive type error : {Key} - {typeof(T)}");
            return Default;
        }
        
        public Dictionary<TK, TV> ReadSubDictionary<TK, TV>(string Key) where TV : IArchiveInfo, new()
        {
            var KeyList = ReadArray<TK>(Key + "_key", null);
            var ValueList = ReadSubArray<TV>(Key + "_value", null);
            if (KeyList == null || ValueList == null || KeyList.Length != ValueList.Length)
            {
                return null;
            }

            var OutputDictionary = new Dictionary<TK, TV>();
            for (var Index = 0; Index < KeyList.Length; ++Index)
            {
                OutputDictionary[KeyList[Index]] = ValueList[Index];
            }

            return OutputDictionary;
        }

        public void Flush()
        {
            while (Stream_.CanRead())
            {
                var (Key, Value) = ReadFromStream(Stream_);
                if (string.IsNullOrWhiteSpace(Key))
                {
                    break;
                }

                CacheList_.Add(Key, Value);
            }
        }

        private (string, object) ReadFromStream(ArchiveReadStream Stream)
        {
            var Key = Stream.ReadString();
            var DataType = (ArchiveDataType) Stream.ReadInt8();

            switch (DataType)
            {
                case ArchiveDataType.Bool:
                    return (Key, Stream.ReadBool());
                case ArchiveDataType.Int8:
                    return (Key, Stream.ReadInt8());
                case ArchiveDataType.Int16:
                    return (Key, Stream.ReadInt16());
                case ArchiveDataType.Int32:
                    return (Key, Stream.ReadInt32());
                case ArchiveDataType.UInt32:
                    return (Key, Stream.ReadUInt32());
                case ArchiveDataType.Int64:
                    return (Key, Stream.ReadInt64());
                case ArchiveDataType.Float:
                    return (Key, Stream.ReadFloat());
                case ArchiveDataType.Double:
                    return (Key, Stream.ReadDouble());
                case ArchiveDataType.String:
                    return (Key, Stream.ReadString());
                case ArchiveDataType.BigInteger:
                    return (Key, Stream.ReadBigInteger());
                case ArchiveDataType.Vector2:
                    return (Key, new UnityEngine.Vector2(Stream.ReadFloat(), Stream.ReadFloat()));
                case ArchiveDataType.Vector3:
                    return (Key, new UnityEngine.Vector3(Stream.ReadFloat(), Stream.ReadFloat(), Stream.ReadFloat()));
                case ArchiveDataType.Vector2Int:
                    return (Key, new UnityEngine.Vector2Int(Stream.ReadInt32(), Stream.ReadInt32()));
                case ArchiveDataType.Vector3Int:
                    return (Key, new UnityEngine.Vector3Int(Stream.ReadInt32(), Stream.ReadInt32(), Stream.ReadInt32()));
                case ArchiveDataType.ArrayBool:
                    return (Key, Stream.ReadArrayBool());
                case ArchiveDataType.ArrayInt32:
                    return (Key, Stream.ReadArrayInt32());
                case ArchiveDataType.Array2Int32:
                    return (Key, Stream.ReadArray2Int32());
                case ArchiveDataType.ArrayBigInteger:
                    return (Key, Stream.ReadArrayBigInteger());
                case ArchiveDataType.Array2BigInteger:
                    return (Key, Stream.ReadArray2BigInteger());
                case ArchiveDataType.ArrayString:
                    return (Key, Stream.ReadArrayString());
                case ArchiveDataType.Array:
                    var Count = Stream.ReadInt16();
                    if (Count == -1)
                    {
                        return (Key, null);
                    }

                    var ArrayT = Type.GetType(Stream.ReadString());
                    var ArrayValue = new object[Count];

                    for (var Index = 0; Index < Count; ++Index)
                    {
                        if (Stream.ReadBool() == false)
                        {
                            ArrayValue[Index] = default;
                        }
                        else
                        {
                            ArrayValue[Index] = ReadFromSubStream(Stream, ArrayT);
                        }
                    }

                    return (Key, ArrayValue);
                case ArchiveDataType.Array2:
                    var Width = Stream.ReadInt16();
                    if (Width == 0)
                    {
                        return (Key, null);
                    }

                    var Height = Stream.ReadInt16();

                    var Array2T = Type.GetType(Stream.ReadString());
                    var Array2Value = new object[Width, Height];

                    for (var X = 0; X < Width; ++X)
                    {
                        for (var Y = 0; Y < Height; ++Y)
                        {
                            if (Stream.ReadBool() == false)
                            {
                                Array2Value[X, Y] = default;
                            }
                            else
                            {
                                Array2Value[X, Y] = ReadFromSubStream(Stream, Array2T);
                            }
                        }
                    }

                    return (Key, Array2Value);
                case ArchiveDataType.Sub:
                    var SubType = Type.GetType(Stream.ReadString());
                    return (Key, ReadFromSubStream(Stream, SubType));
                default:
                    break;
            }

            return (string.Empty, null);
        }

        private IArchiveInfo ReadFromSubStream(ArchiveReadStream MainStream, Type SubType)
        {
            var SubStream = MainStream.ReadStream();
            var SubEncoder = new ArchiveDecoder(SubStream);
            SubEncoder.Flush();
            var SubInfo = Activator.CreateInstance(SubType) as IArchiveInfo;
            SubInfo.Decode(SubEncoder);
            return SubInfo;
        }
    }
}