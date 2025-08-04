using FFmpeg.AutoGen;
using MFFAmpeg.AVBuffers;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Context;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMPacketWriter"/> </summary>
internal class MPacketWriter : MFormatOperation, IMPacketWriter
{
    public long CurrentTimestamp { get { return _timeInfo.Timestamp; } }


    private int _index = -1;

    private MAudioStreamFormat _format;

    private MTimestamp _timeInfo = new MTimestamp();

    internal MPacketWriter(int fferror) : base(fferror) { }

    internal unsafe MPacketWriter(MFormatContext formatContext, int index, MAudioStreamFormat format)
        : base(formatContext)
    {
        _index = index;
        _format = format;
    }

    public unsafe int Write(MPacket packet)
    {
        return ffmpeg.av_write_frame(_context, packet.Packet);
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
        packet.Dispose();
        if (result >= 0)
        {
            _timeInfo._timestamp += sampleCount;
        }
        return result;
    }
}
