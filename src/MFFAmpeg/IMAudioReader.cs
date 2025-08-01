using MFFAmpeg.AVFormats;

namespace MFFAmpeg;


/// <summary> Current implementation is using FFmpeg to read only main audio stream from a WAV file. </summary>
public interface IMAudioReader : IMFFmpegOperation, IDisposable
{
    MAudioFileFormat FileFormat { get; }

    MAudioStreamFormat StreamFormat { get; }

    IMPacketReader OpenMainStream();
}
