using MFFAmpeg.AVFormats;

namespace MFFAmpeg;


/// <summary> Current implementation only supports writting 16-bit native LE PCM to a WAV file. </summary>
public interface IMAudioWriter : IMFFmpegOperation, IDisposable
{
    /// <summary>
    /// Stream needs to be added before <see cref="StartPacketWriter"/> is invoked.
    /// </summary>
    /// <param name="streamFormat"></param>
    /// <returns></returns>
    int AddStream(MAudioStreamFormat streamFormat);

    IMPacketWriter StartPacketWriter();
}
