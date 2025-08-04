using MFFAmpeg.Context;

namespace MFFAmpeg;

public interface IMFormatOperation : IMFFmpegOperation
{
    /// <summary> Used by class of FFmpeg operations that require <see cref="FFmpeg.AutoGen.AVFormatContext"/></summary>
    MFormatContext Context { get; }
}
