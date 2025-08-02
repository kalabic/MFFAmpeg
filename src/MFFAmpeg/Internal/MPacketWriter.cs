using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;
using System;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMPacketWriter"/> </summary>
internal class MPacketWriter : UnmanagedFormatContext, IMPacketWriter
{
    public IMTimestamp? TimeInfo { get { return null; } }


    private int _index = -1;

    private MAudioStreamFormat _format;

    private MTimestamp _timeInfo = new MTimestamp();

    internal MPacketWriter(int fferror) : base(fferror) { }

    internal unsafe MPacketWriter(AVFormatContext* formatContext, int index, MAudioStreamFormat format)
        : base(formatContext)
    {
        _index = index;
        _format = format;
    }

    public unsafe int Write(MPacket packet)
    {
        return ffmpeg.av_write_frame(_formatContext, packet.Packet);
    }

    public int WritePacketFromData(MByteBuffer data)
    {
        var sampleCount = (long)_format.ByteToSampleCount(data.BytesUsed);
        var packet = new MPacket(data);
        packet.StreamIndex = _index;
        packet.DTS = _timeInfo.Timestamp;
        packet.Duration = sampleCount;
        packet.PTS = _timeInfo.Timestamp;
        var result = Write(packet);
        if (result >= 0)
        {
            _timeInfo._timestamp += sampleCount;
        }
        return result;
    }
}
