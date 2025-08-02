using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Base;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMAudioWriter"/> </summary>
internal unsafe class MAudioWriter : ManagedFormatContext, IMAudioWriter
{
    internal static unsafe MAudioWriter Open(string path, MAudioFileFormat fileFormat, CancellationToken cancellation = default)
    {
        void* it = null;
        AVOutputFormat* outputFormat;
        bool wavMuxerFound = false;
        while (true)
        {
            outputFormat = ffmpeg.av_muxer_iterate(&it);
            if (outputFormat == null)
            {
                break;
            }

            if (outputFormat->audio_codec == fileFormat._codec_id && outputFormat->extensions is not null)
            {
                var outputExtension = FFmpegHelper.ToString(outputFormat->extensions);
                if (outputExtension is not null && outputExtension.Equals(fileFormat._extension))
                {
                    wavMuxerFound = true;
                    break;
                }
            }
        }

        if (!wavMuxerFound)
        {
            return new MAudioWriter(ffmpeg.AVERROR_MUXER_NOT_FOUND);
        }

        int fferror = 0;
        AVFormatContext* formatContext;
        fferror = ffmpeg.avformat_alloc_output_context2(&formatContext, outputFormat, "", path);
        if (fferror < 0)
        {
            return new MAudioWriter(fferror);
        }

        AVIOContext* ioContext;
        fferror = ffmpeg.avio_open2(&ioContext, path, ffmpeg.AVIO_FLAG_WRITE, null, null);
        if (fferror < 0)
        {
            ffmpeg.avformat_free_context(formatContext);
            return new MAudioWriter(fferror);
        }
        formatContext->pb = ioContext;

        return new MAudioWriter(fileFormat, formatContext, cancellation);
    }

    private readonly MAudioFileFormat _fileFormat;

    private readonly List<MPacketWriter> _streamList = [];

    private bool _headerWritten = false;

    private bool _trailerWritten = false;

    internal MAudioWriter(int fferror)
        : base(fferror)
    {
    }

    internal MAudioWriter(MAudioFileFormat fileFormat, AVFormatContext* formatContext, CancellationToken cancellation = default)
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
        if (disposing)
        {
            if (_headerWritten && !_trailerWritten)
            {
                Stop();
            }

            foreach(var stream in _streamList)
            {
                stream.Dispose();
            }
            _streamList.Clear();
        }
        base.Dispose(disposing);
    }

    public int AddStream(MAudioStreamFormat streamFormat)
    {
        if (streamFormat._bits_per_sample != 16)
        {
            throw new ArgumentException($"Given bits per sample value is not supported ({streamFormat._bits_per_sample}).");
        }

        if (streamFormat._num_channels < 1 || streamFormat._num_channels > 2)
        {
            throw new ArgumentException($"Given number of channels is not supported ({streamFormat._num_channels}).");
        }

        AVStream* stream = ffmpeg.avformat_new_stream(_formatContext, null);
        if (stream is null)
        {
            return ffmpeg.AVERROR_UNKNOWN;
        }

        stream->codecpar->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
        stream->codecpar->codec_id = _fileFormat._codec_id;
        stream->codecpar->format = (int)_fileFormat._format;
        stream->codecpar->bits_per_coded_sample = streamFormat._bits_per_sample;
        stream->codecpar->sample_rate = streamFormat._sample_rate;
        stream->codecpar->ch_layout.nb_channels = streamFormat._num_channels;

        _streamList.Add(new MPacketWriter(_formatContext, stream->index, streamFormat));

        return 0;
    }

    public IMPacketWriter StartPacketWriter()
    {
        if (IsCancelled)
        {
            return new MPacketWriter(ffmpeg.AVERROR_EXIT);
        }

        if (ContextIsNotValid || _streamList.Count == 0)
        {
            return new MPacketWriter(ffmpeg.AVERROR_EXTERNAL);
        }

        _fferror = ffmpeg.avformat_write_header(_formatContext, null);
        if (_fferror < 0)
        {
            return new MPacketWriter(_fferror);
        }

        _headerWritten = true;

        return _streamList[0];
    }

    public int Stop()
    {
        if (ContextIsNotValid)
        {
            return ffmpeg.AVERROR_EXTERNAL;
        }

        _fferror = ffmpeg.av_write_trailer(_formatContext);
        _trailerWritten = true;
        return _fferror;
    }
}
