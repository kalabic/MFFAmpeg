namespace MFFAmpeg;


public interface IMFFmpegOperation : IDisposable
{
    /// <summary> Tells if object implementing FFmpeg operations has entered error state or not. </summary>
    int FFerror { get; }

    /// <summary> State of <see cref="CancellationToken"/> object was created with. </summary>
    bool IsCancelled { get; }
}
