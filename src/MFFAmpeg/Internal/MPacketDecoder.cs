using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Context;

namespace MFFAmpeg.Internal;


/// <summary> See summary for <see cref="IMPacketDecoder"/> </summary>
internal class MPacketDecoder : MCodecOperation, IMPacketDecoder
{
    internal static unsafe MPacketDecoder Create(MCodec codec, 
                                                 MCodecParameters parameters,
                                                 CancellationToken cancellation = default)
    {
        MCodecContext? codecContext = MCodecContext.Create(codec, parameters);
        if (codecContext is null || codecContext.IsNotValid)
        {
            return new MPacketDecoder(ffmpeg.AVERROR_EXTERNAL);
        }
        else
        {
           return new MPacketDecoder(codecContext, cancellation);
        }
    }

    public MAudioStreamFormat AudioSampleFormat { get { return _audioSampleFormat; } }

    private MAudioStreamFormat _audioSampleFormat;

    internal MPacketDecoder(int fferror) : base(fferror) { }

    internal unsafe MPacketDecoder(MCodecContext codecContext, 
                                   CancellationToken cancellation = default)
        : base(codecContext, cancellation)
    {
        var cc = codecContext.Ptr;
        _audioSampleFormat = new MAudioStreamFormat(cc->sample_fmt, cc->sample_rate, cc->bits_per_raw_sample, cc->ch_layout.nb_channels);
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        void DisposeResources()
        {
            _context.Dispose();
        }

        if (disposing)
        {
            DisposeResources();
        }
        else
        {
#if DEBUG_UNDISPOSED
            if (_context.IsNotNull)
            {
                throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
            }
#else
            DisposeResources();
#endif
        }
        base.Dispose(disposing);
    }


    /// <summary> See summary for <see cref="IMPacketDecoder.SendPacket(MPacket)"/> </summary>
    public unsafe int SendPacket(MPacket packet)
    {
        return ffmpeg.avcodec_send_packet(_context, packet);
    }


    /// <summary> See summary for <see cref="IMPacketDecoder.ReceiveFrame(MFrame)"/> </summary>
    public unsafe int ReceiveFrame(MFrame frame)
    {
        return ffmpeg.avcodec_receive_frame(_context, frame);
    }
}
