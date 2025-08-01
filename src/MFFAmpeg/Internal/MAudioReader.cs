using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMAudioReader"/> </summary>
internal unsafe class MAudioReader : ManagedFormatContext, IMAudioReader
{
    internal static MAudioReader Open(string path, MInputFormat? ifmt, CancellationToken cancellation = default)
    {
        int fferror = 0;

        AVFormatContext* formatContext;
        formatContext = ffmpeg.avformat_alloc_context();
        fferror = ffmpeg.avformat_open_input(&formatContext, path, ifmt is null ? null : ifmt.Format, null);
        if (fferror < 0)
        {
            return new MAudioReader(fferror, path);
        }

        var streamIndex = ffmpeg.av_find_best_stream(formatContext, AVMediaType.AVMEDIA_TYPE_AUDIO, -1, -1, null, 0);
        if (streamIndex < 0)
        {
            // streamIndex is non-negative stream number in case of success, but in case of error it is
            // AVERROR_STREAM_NOT_FOUND if no stream with the requested type could be found,
            // AVERROR_DECODER_NOT_FOUND if streams were found but no decoder.
            return new MAudioReader(streamIndex, path);
        }

        return new MAudioReader(path, formatContext, streamIndex, cancellation);
    }

    public MAudioFileFormat FileFormat
    {
        get { return new MAudioFileFormat("", 0, _codecPar->codec_id); }
    }

    public MAudioStreamFormat StreamFormat
    {
        get { return new MAudioStreamFormat(_codecPar->sample_rate, _codecPar->bits_per_coded_sample, _codecPar->ch_layout.nb_channels); }
    }

    private readonly string _path;

    private readonly int _streamIndex;

    private readonly AVCodecParameters* _codecPar;

    private MPacketReader? _streamReader = null;

    internal MAudioReader(int fferror, string path)
        : base(fferror)
    {
        _path = path;
    }

    internal MAudioReader(string path, AVFormatContext* formatContext, int streamIndex, CancellationToken cancellation = default)
        : base(formatContext, cancellation)
    {
        _path = path;
        _streamIndex = streamIndex;
        _codecPar = formatContext->streams[streamIndex]->codecpar;
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _streamReader?.Dispose();
        }
        base.Dispose(disposing);
    }

    public IMPacketReader OpenMainStream()
    {
        if (_streamReader is null)
        {
            if (!IsCancelled && !FormatContextIsNull)
            {
                _streamReader = new MPacketReader(_formatContext, _streamIndex, _cancellation);
            }
            else
            {
                _streamReader = new MPacketReader(IsCancelled ? ffmpeg.AVERROR_EXIT : ffmpeg.AVERROR_EXTERNAL);
            }
        }
        else
        {
            throw new NotImplementedException("Cannot open multiple instances of a packet reader.");
        }
        return _streamReader;
    }
}
