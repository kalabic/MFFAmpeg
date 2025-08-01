using FFmpeg.AutoGen;

namespace MFFAmpeg.Internal;

internal abstract class UnmanagedFormatContext : FFmpegOperation
{
    internal UnmanagedFormatContext(int fferror) : base(fferror) { }

    internal unsafe UnmanagedFormatContext(AVFormatContext* formatContext, CancellationToken cancellation = default) 
        : base(formatContext, cancellation)
    { }
}
