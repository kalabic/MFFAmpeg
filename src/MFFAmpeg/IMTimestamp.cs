namespace MFFAmpeg;


/// <summary>
/// Equivalent of timestamps found in FFmpeg's AVPacket class.
/// </summary>
public interface IMTimestamp
{
    long Timestamp { get; }

    long Duration { get; }
}
