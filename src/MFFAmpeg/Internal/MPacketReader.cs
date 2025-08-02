using FFmpeg.AutoGen;
using System.Collections;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMPacketReader"/> </summary>
internal class MPacketReader : UnmanagedFormatContext, IMPacketReader
{
    internal unsafe class MPacketEnumerator : UnmanagedFormatContext, IEnumerator<MPacket>
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

        internal MPacketEnumerator(AVFormatContext* formatContext, int streamIndex, CancellationToken cancellation = default)
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
                _fferror = ffmpeg.av_read_frame(_formatContext, packet);
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
            if (IsCancelled || ContextIsNotValid)
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
    

    public IMTimestamp? TimeInfo { get { return null; } }



    private readonly int _streamIndex;

    private MPacketEnumerator? _enumerator;

    internal MPacketReader(int fferror) : base(fferror) { }

    internal unsafe MPacketReader(AVFormatContext* formatContext, int streamIndex, CancellationToken cancellation = default)
        : base(formatContext, cancellation)
    {
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
        if (ContextIsNotValid)
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
            int fferror = ffmpeg.avformat_seek_file(_formatContext, 0, 0, 0, 0, ffmpeg.AVSEEK_FLAG_BACKWARD);
            if (fferror >= 0)
            {
                _enumerator = new MPacketEnumerator(_formatContext, _streamIndex);
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
