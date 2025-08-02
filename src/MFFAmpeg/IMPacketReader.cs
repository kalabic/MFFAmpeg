namespace MFFAmpeg;


/// <summary>
/// Use <see cref="IEnumerator{MPacket}"/> interface returned by <see cref="IEnumerable{MPacket}.GetEnumerator"/>
/// to read a stream of <see cref="MPacket"/> from input.
/// </summary>
public interface IMPacketReader : IEnumerable<MPacket>, IMFFmpegOperation
{
    /// <summary>
    /// Packet reader may or may not support providing timestamp information.
    /// </summary>
    IMTimestamp? TimeInfo { get; }
}
