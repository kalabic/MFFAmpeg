namespace MFFAmpeg;


/// <summary>
/// WIP
/// </summary>
public interface IMPacketWriter : IMFFmpegOperation
{
    /// <summary>
    /// Packet writer may or may not support providing timestamp information.
    /// </summary>
    IMTimestamp? TimeInfo { get; }

    /// <summary>
    /// Write a packet of data.
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    int Write(MPacket packet);

    /// <summary>
    /// Write a packet of data using data buffer allocated with <see cref="MFFApi.AllocPacketBuffer(ulong)"/>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    int WritePacketFromData(MByteBuffer data);
}
