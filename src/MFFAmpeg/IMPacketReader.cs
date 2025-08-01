namespace MFFAmpeg;


/// <summary>
/// Use <see cref="IEnumerator{MPacket}"/> interface returned by <see cref="IEnumerable{MPacket}.GetEnumerator"/>
/// to read a stream of <see cref="MPacket"/> from input.
/// </summary>
public interface IMPacketReader : IEnumerable<MPacket>, IMFFmpegOperation
{
}
