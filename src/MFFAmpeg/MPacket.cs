using DotBase.Core;
using FFmpeg.AutoGen;
using MFFAmpeg.AVBuffers;

namespace MFFAmpeg;


/// <summary>
/// Presumably safe wrapper for <see cref="AVPacket"/>
/// </summary>
public unsafe class MPacket : DisposableBase
{
    public static implicit operator AVPacket*(MPacket packet) { return packet._packet; }


    /// <summary> See <see cref="AVPacket.data"/> </summary>
    public byte* Data { get { return _packet->data; } }


    /// <summary> See <see cref="AVPacket.dts"/> </summary>
    public long DTS { get { return _packet->dts; } set { _packet->dts = value; } }


    /// <summary> See <see cref="AVPacket.duration"/> </summary>
    public long Duration { get { return _packet->duration; } set { _packet->duration = value; } }


    /// <summary> See <see cref="AVPacket.pts"/> </summary>
    public long PTS { get { return _packet->pts; } set { _packet-> pts = value; } }


    /// <summary> See <see cref="AVPacket.size"/> </summary>
    public int Size { get { return _packet->size; } }


    /// <summary> See <see cref="AVPacket.stream_index"/> </summary>
    public int StreamIndex { get { return _packet->stream_index; } set { _packet->stream_index = value; } }


    /// <summary> Indicates if this is last packet available in input stream or not. </summary>
    public bool LastPacket { get { return _lastPacket; } }


    /// <summary> Direct access to <see cref="AVPacket"/> managed by instance of <see cref="MPacket"/></summary>
    public AVPacket* Packet { get { return _packet; } }


    //
    // Private implementation details begin below.
    //

    
    private AVPacket* _packet = null;

    private bool _lastPacket = false;

    public MPacket(MByteBuffer buffer)
    {
        AVPacket* packet = ffmpeg.av_packet_alloc();
        if (ffmpeg.av_packet_from_data(packet, buffer.Data, (int)buffer.BytesUsed) == 0)
        {
            _packet = packet;
            buffer.ReleaseData();
        }
        else
        {
            ffmpeg.av_packet_free(&packet);
        }
    }

    public MPacket(AVPacket* packet, bool lastPacket = false)
    {
        if (packet is not null)
        {
            _packet = ffmpeg.av_packet_clone(packet);
        }

        _lastPacket = lastPacket;
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        void DisposeResources()
        {
            fixed (AVPacket** packet = &_packet)
            {
                ffmpeg.av_packet_free(packet);
            }
            _packet = null;
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
#else
            DisposeResources();
#endif
        }
        base.Dispose(disposing);
    }
}
