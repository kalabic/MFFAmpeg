using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Context;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMAudioWriter"/> </summary>
internal unsafe class MAudioWriter : MFormatOperation, IMAudioWriter
{
    internal static unsafe MAudioWriter Open(string path, MAudioFileFormat fileFormat, CancellationToken cancellation = default)
    {
        MOutputFormat format = MFFApi.FindMuxerForFileFormat(fileFormat);
        if (format.IsNull)
        {
            return new MAudioWriter(ffmpeg.AVERROR_MUXER_NOT_FOUND);
        }

        int fferror = 0;
        MFormatContext? context = MFormatContext.CreateOutput(path, format, ref fferror);
        if (context is null)
        {
            return new MAudioWriter(fferror);
        }

        return new MAudioWriter(fileFormat, context, cancellation);
    }

    private readonly MAudioFileFormat _fileFormat;

    private readonly List<MPacketWriter> _streamList = [];

    private bool _headerWritten = false;

    private bool _trailerWritten = false;

    internal MAudioWriter(int fferror)
        : base(fferror)
    {
    }

    internal MAudioWriter(MAudioFileFormat fileFormat, MFormatContext formatContext, CancellationToken cancellation = default)
        : base(formatContext, cancellation)
    {
        _fileFormat = fileFormat;
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        void DisposeResources()
        {
            if (_headerWritten && !_trailerWritten)
            {
                Stop();
            }

            foreach (var stream in _streamList)
            {
                stream.Dispose();
            }
            _streamList.Clear();
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

    public int AddStream(MAudioStreamFormat streamFormat)
    {
        AVStream* stream = ffmpeg.avformat_new_stream(_context, null);
        if (stream is null)
        {
            return ffmpeg.AVERROR_UNKNOWN;
        }

        stream->codecpar->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
        stream->codecpar->codec_id = _fileFormat.CodecId;
        stream->codecpar->format = (int)_fileFormat.Format;
        stream->codecpar->bits_per_coded_sample = streamFormat.BitsPerSample;
        stream->codecpar->sample_rate = streamFormat.SampleRate;
        stream->codecpar->ch_layout.nb_channels = streamFormat.NumChannels;

        _streamList.Add(new MPacketWriter(_context, stream->index, streamFormat));

        return 0;
    }

    public IMPacketWriter StartPacketWriter()
    {
        if (IsCancelled)
        {
            return new MPacketWriter(ffmpeg.AVERROR_EXIT);
        }

        if (Context.IsNotValid || _streamList.Count == 0)
        {
            return new MPacketWriter(ffmpeg.AVERROR_EXTERNAL);
        }

        _fferror = ffmpeg.avformat_write_header(_context, null);
        if (_fferror < 0)
        {
            return new MPacketWriter(_fferror);
        }

        _headerWritten = true;

        return _streamList[0];
    }

    public int Stop()
    {
        if (Context.IsNotValid)
        {
            return ffmpeg.AVERROR_EXTERNAL;
        }

        _fferror = ffmpeg.av_write_trailer(_context);
        _trailerWritten = true;
        return _fferror;
    }
}
