namespace MFFAmpeg;

public interface IMPacketWriter : IMFFmpegOperation
{
    /// <summary>
    /// Allocate new data buffer for a packet to be written using <see cref="Write(MPacket)"/>
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    MByteBuffer AllocPacketBuffer(ulong size);

    /// <summary>
    /// Write a packet of data, with packet buffer previously allocated using <see cref="AllocPacketBuffer(ulong)"/>
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    int Write(MPacket packet);
}
