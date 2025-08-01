namespace MFFAmpeg;

/// <summary>
/// Tells if object implementing FFmpeg operations has entered error state or not.
/// </summary>
public interface IMFFmpegOperation
{
    int FFerror { get; }

    bool IsCancelled { get; }
}
