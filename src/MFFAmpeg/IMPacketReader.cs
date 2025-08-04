using MFFAmpeg.AVFormats;

namespace MFFAmpeg;


/// <summary>
/// Use <see cref="IEnumerator{MPacket}"/> interface returned by <see cref="IMPacketReader"/>.
/// to read a stream of <see cref="MPacket"/> from input.
/// <para>For example: foreach(var packet in packetReader) { ... }</para>
/// </summary>
public interface IMPacketReader : IEnumerable<MPacket>, IMFFmpegOperation
{
    MCodec Codec { get; }

    MCodecParameters CodecParameters { get; }

    MAudioStreamFormat StreamFormat { get; }

    /// <summary>
    /// Packet reader may or may not support providing timestamp information.
    /// </summary>
    IMTimestamp? TimeInfo { get; }
}
