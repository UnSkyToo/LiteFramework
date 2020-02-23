namespace LiteFramework
{
    public delegate void LiteAction();

    public delegate void LiteAction<in T>(T Arg);

    public delegate void LiteAction<in T1, in T2>(T1 Arg1, T2 Arg2);

    public delegate void LiteAction<in T1, in T2, in T3>(T1 Arg1, T2 Arg2, T3 Arg3);

    public delegate void LiteAction<in T1, in T2, in T3, in T4>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4);

    public delegate void LiteAction<in T1, in T2, in T3, in T4, in T5>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5);

    public delegate void LiteAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6);

    public delegate void LiteAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7);

    public delegate void LiteAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7, T8 Arg8);

    public delegate void LiteAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7, T8 Arg8, T9 Arg9);

    public delegate void LiteAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7, T8 Arg8, T9 Arg9, T10 Arg10);

    public delegate TResult LiteFunc<out TResult>();

    public delegate TResult LiteFunc<in T, out TResult>(T Arg);

    public delegate TResult LiteFunc<in T1, in T2, out TResult>(T1 Arg1, T2 Arg2);

    public delegate TResult LiteFunc<in T1, in T2, in T3, out TResult>(T1 Arg1, T2 Arg2, T3 Arg3);

    public delegate TResult LiteFunc<in T1, in T2, in T3, in T4, out TResult>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4);

    public delegate TResult LiteFunc<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5);

    public delegate TResult LiteFunc<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6);

    public delegate TResult LiteFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7);

    public delegate TResult LiteFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7, T8 Arg8);

    public delegate TResult LiteFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, out TResult>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7, T8 Arg8, T9 Arg9);

    public delegate TResult LiteFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, out TResult>(T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7, T8 Arg8, T9 Arg9, T10 Arg10);
}