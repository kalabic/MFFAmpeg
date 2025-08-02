namespace MFFAmpeg.Internal;

internal class MPacketList
{
    internal static IList<MPacket> Create(IMPacketReader inputStream)
    {
        return new List<MPacket>(inputStream);
    }
}
