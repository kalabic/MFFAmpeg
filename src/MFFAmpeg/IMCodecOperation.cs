using MFFAmpeg.Context;

namespace MFFAmpeg;

public interface IMCodecOperation : IMFFmpegOperation
{
    /// <summary> Used by class of FFmpeg operations that require <see cref="FFmpeg.AutoGen.AVCodecContext"/></summary>
    MCodecContext Context { get; }
}
