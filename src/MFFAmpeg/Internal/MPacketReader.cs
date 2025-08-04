using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Context;
using System.Collections;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMPacketReader"/> </summary>
internal class MPacketReader : MFormatOperation, IMPacketReader
{
    internal unsafe class MPacketEnumerator : MFormatOperation, IEnumerator<MPacket>
    {
        public MPacket Current { get { return new MPacket(_packet, (_nextPacket == null)); } }

        object IEnumerator.Current => Current;

        private readonly int _streamIndex;

        private AVPacket* _packet;

        private AVPacket* _nextPacket;

        internal MPacketEnumerator(int fferror)
            : base(fferror)
        {
            _streamIndex = 0;
            _packet = null;
            _nextPacket = null;
        }

        internal MPacketEnumerator(MFormatContext formatContext, int streamIndex, CancellationToken cancellation = default)
            : base(formatContext, cancellation)
        {
            _streamIndex = streamIndex;
            _packet = null;
            _nextPacket = ReadInternal();
        }

        AVPacket* ReadInternal()
        {
            AVPacket* packet = null;
            while (!IsCancelled)
            {
                packet = ffmpeg.av_packet_alloc();
                _fferror = ffmpeg.av_read_frame(_context, packet);
                if (_fferror < 0)
                {
                    ffmpeg.av_packet_free(&packet);
                    return null;
                }

                if (packet->stream_index != _streamIndex)
                {
                    ffmpeg.av_packet_free(&packet);
                    continue;
                }

                break;
            }
            return packet;
        }

        protected override void Dispose(bool disposing)
        {
            void DisposeResources()
            {
                if (_packet is not null)
                {
                    fixed (AVPacket** packet = &_packet)
                    {
                        ffmpeg.av_packet_free(packet);
                    }
                    _packet = null;
                }

                if (_nextPacket is not null)
                {
                    fixed (AVPacket** packet = &_nextPacket)
                    {
                        ffmpeg.av_packet_free(packet);
                    }
                    _nextPacket = null;
                }
            }

            if (disposing)
            {
                DisposeResources();
            }
            else
            {
#if DEBUG_UNDISPOSED
                if (_packet is not null)
                {
                    throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
                }

                if (_nextPacket is not null)
                {
                    throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
                }
#else
                DisposeResources();
#endif
            }
            base.Dispose(disposing);
        }

        public bool MoveNext()
        {
            if (IsCancelled || Context.IsNotValid)
            {
                return false;
            }

            if (_packet is not null)
            {
                fixed(AVPacket** packet = &_packet)
                {
                    ffmpeg.av_packet_free(packet);
                }
            }
            _packet = _nextPacket;
            _nextPacket = ReadInternal();
            return (_packet is not null);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public MCodec Codec { get { return _codec; } }

    public MCodecParameters CodecParameters { get { return _codecParameters; } }

    public MAudioStreamFormat StreamFormat { get { return _codecParameters.StreamFormat; } }

    public IMTimestamp? TimeInfo { get { return null; } }

    private MCodec _codec;

    private MCodecParameters _codecParameters;

    private readonly int _streamIndex;

    private MPacketEnumerator? _enumerator;

    internal MPacketReader(int fferror) 
        : base(fferror) 
    {
        _codec = new MCodec();
        _codecParameters = new MCodecParameters();
        _streamIndex = -1;
    }

    internal unsafe MPacketReader(MFormatContext context, int streamIndex, MCodec codec, CancellationToken cancellation = default)
        : base(context, cancellation)
    {
        _codec = codec;
        _codecParameters = new MCodecParameters(Context.Ptr->streams[streamIndex]->codecpar);
        _streamIndex = streamIndex;
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        void DisposeResources()
        {
            if (_enumerator is not null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
        }

        if (disposing)
        {
            DisposeResources();
        }
        else
        {
#if DEBUG_UNDISPOSED
            if (_enumerator is not null)
            {
                throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
            }
#else
            DisposeResources();
#endif
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Current implementation does not support having multiple enumerators at a time.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<MPacket> GetEnumerator()
    {
        if (Context.IsNotValid)
        {
            return new MPacketEnumerator(ffmpeg.AVERROR_EXTERNAL);
        }

        if (IsCancelled)
        {
            return new MPacketEnumerator(ffmpeg.AVERROR_EXIT);
        }

        if (_enumerator is not null)
        {
            _enumerator.Dispose();
            _enumerator = null;
        }

        unsafe
        {
            int fferror = ffmpeg.avformat_seek_file(_context, 0, 0, 0, 0, ffmpeg.AVSEEK_FLAG_BACKWARD);
            if (fferror >= 0)
            {
                _enumerator = new MPacketEnumerator(_context, _streamIndex);
            }
            else
            {
                _enumerator = new MPacketEnumerator(fferror);
            }
        }

        return _enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
