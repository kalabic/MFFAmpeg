using MFFAmpeg.AVFormats;

namespace MFFAmpeg;

/// <summary>
/// Method <see cref="AddStream(MAudioStreamFormat)"/> must be used to add at least one stream before
/// <see cref="StartPacketWriter"/> can be invoked to return valid packet writter interface.
/// </summary>
public interface IMAudioWriter : IMFFmpegOperation, IDisposable
{
    /// <summary>
    /// At least one stream needs to be added before <see cref="StartPacketWriter"/> is invoked.
    /// </summary>
    /// <param name="streamFormat"></param>
    /// <returns></returns>
    int AddStream(MAudioStreamFormat streamFormat);


    /// <summary>
    /// Whether anything actually is written to the output file depends on the muxer, but this function must always be called.
    /// </summary>
    /// <returns></returns>
    IMPacketWriter StartPacketWriter();
}
