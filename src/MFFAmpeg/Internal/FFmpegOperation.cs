using FFmpeg.AutoGen;
using MFFAmpeg.Base;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMFFmpegOperation"/> </summary>
internal abstract unsafe class FFmpegOperation : DisposableBase, IMFFmpegOperation
{
    public int FFerror { get {  return _fferror; } }

    public bool IsCancelled { get { return _cancellation.IsCancellationRequested; } }

    public bool FormatContextIsNull { get { return _formatContext is null; } }

    protected AVFormatContext* _formatContext;

    protected int _fferror;

    protected CancellationToken _cancellation;

    internal FFmpegOperation(int fferror)
    {
        _fferror = fferror;
        _formatContext = null;
        _cancellation = default;
    }

    internal FFmpegOperation(AVFormatContext* formatContext, CancellationToken cancellation = default)
    {
        _fferror = 0;
        _formatContext = formatContext;
        _cancellation = cancellation;
    }
}
