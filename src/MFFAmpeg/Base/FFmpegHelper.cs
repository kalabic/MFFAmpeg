using System.Runtime.InteropServices;

namespace MFFAmpeg.Base;


public static class FFmpegHelper
{
    internal static unsafe string? ToString(byte* source)
    {
        return Marshal.PtrToStringAnsi((nint)source);
    }

    public static int ThrowIfError(this int error)
    {
        if (error < 0) throw new ApplicationException(MFFApi.av_strerror(error));
        return error;
    }

    public static T ThrowIfError<T>(this T handle) where T : IMFFmpegOperation
    {
        if (handle.FFerror < 0) throw new ApplicationException(MFFApi.av_strerror(handle.FFerror));
        return handle;
    }
}
