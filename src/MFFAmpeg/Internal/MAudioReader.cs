using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Context;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMAudioReader"/> </summary>
internal unsafe class MAudioReader : MFormatOperation, IMAudioReader
{
    internal static MAudioReader Open(string path, MInputFormat? format, CancellationToken cancellation = default)
    {
        var context = MFormatContext.Create();
        if (context is null)
        {
            return new MAudioReader(ffmpeg.AVERROR_UNKNOWN, path);
        }

        AVInputFormat* inputFormat = (format is null) ? null : (AVInputFormat *)format;
        int fferror = 0;
        fferror = ffmpeg.avformat_open_input(context, path, inputFormat, null);
        if (fferror < 0)
        {
            context.Dispose();
            return new MAudioReader(fferror, path);
        }

        fferror = ffmpeg.avformat_find_stream_info(context, null);
        if (fferror < 0)
        {
            context.Dispose();
            return new MAudioReader(fferror, path);
        }

        AVCodec* codec = null;
        var streamIndex = ffmpeg.av_find_best_stream(context, AVMediaType.AVMEDIA_TYPE_AUDIO, -1, -1, &codec, 0);
        if (streamIndex < 0)
        {
            context.Dispose();
            // streamIndex is non-negative stream number in case of success, but in case of error it is
            // AVERROR_STREAM_NOT_FOUND if no stream with the requested type could be found,
            // AVERROR_DECODER_NOT_FOUND if streams were found but no decoder.
            return new MAudioReader(streamIndex, path);
        }

        return new MAudioReader(path, context, streamIndex, codec, cancellation);
    }

    public string Path { get { return _path; } }


    private readonly string _path;

    private readonly int _streamIndex;

    private readonly MCodec _codec;

    private MPacketReader? _reader = null;

    internal MAudioReader(int fferror, string path)
        : base(fferror)
    {
        _path = path;
        _codec = new MCodec(null);
    }

    internal MAudioReader(string path, MFormatContext context, int streamIndex, AVCodec* streamCodec, CancellationToken cancellation = default)
        : base(context, cancellation)
    {
        _path = path;
        _streamIndex = streamIndex;
        _codec = new MCodec(streamCodec);
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        void DisposeResources()
        {
            _reader?.Dispose();
            _context.Dispose();
        }

        if (disposing)
        {
            DisposeResources();
        }
        else
        {
#if DEBUG_UNDISPOSED
            if (_context.Ptr is not null)
            {
                throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
            }
#else
            DisposeResources();
#endif
        }

        base.Dispose(disposing);
    }

    public IMPacketReader OpenMainStream()
    {
        if (_reader is null)
        {
            if (!IsCancelled && !Context.IsNotValid)
            {
                _reader = new MPacketReader(_context, _streamIndex, _codec, _cancellation);
            }
            else
            {
                _reader = new MPacketReader(IsCancelled ? ffmpeg.AVERROR_EXIT : ffmpeg.AVERROR_EXTERNAL);
            }
        }
        return _reader;
    }
}
