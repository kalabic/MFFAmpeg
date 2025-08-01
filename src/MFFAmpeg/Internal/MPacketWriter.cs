using FFmpeg.AutoGen;

namespace MFFAmpeg.Internal;


/// <summary> See comments for <see cref="IMPacketWriter"/> </summary>
internal class MPacketWriter : UnmanagedFormatContext, IMPacketWriter
{
    public MPacketWriter(int fferror) : base(fferror) { }

    public unsafe MPacketWriter(AVFormatContext* formatContext) : base(formatContext) { }

    public MByteBuffer AllocPacketBuffer(ulong size)
    {
        return new MByteBuffer(size);
    }

    public unsafe int Write(MPacket packet)
    {
        return (!FormatContextIsNull) ? ffmpeg.av_write_frame(_formatContext, packet.Packet) : ffmpeg.AVERROR_EXTERNAL;
    }
}
