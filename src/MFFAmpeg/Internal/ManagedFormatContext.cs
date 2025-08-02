using FFmpeg.AutoGen;

namespace MFFAmpeg.Internal;

internal abstract class ManagedFormatContext : FFmpegOperation
{
    internal ManagedFormatContext(int fferror) : base(fferror) { }

    internal unsafe ManagedFormatContext(AVFormatContext* formatContext, CancellationToken cancellation = default) 
        : base(formatContext, cancellation) 
    { }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        void DisposeResources()
        {
            unsafe
            {
                fixed (AVFormatContext** ppContext = &_formatContext)
                {
                    if (_formatContext->pb is not null)
                    {
                        ffmpeg.avformat_close_input(ppContext);
                    }
                    else
                    {
                        ffmpeg.avformat_free_context(_formatContext);
                        _formatContext = null;
                    }
                }
            }
        }

        if (disposing)
        {
            DisposeResources();
        }
        else
        {
            if (!ContextIsNotValid)
            {
#if DEBUG_UNDISPOSED
                throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
#else
                DisposeResources();
#endif
            }
        }
        base.Dispose(disposing);
    }
}
