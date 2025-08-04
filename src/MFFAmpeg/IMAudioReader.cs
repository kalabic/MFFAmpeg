namespace MFFAmpeg;


/// <summary> Current implementation is using FFmpeg to read only main audio stream from source. </summary>
public interface IMAudioReader : IMFFmpegOperation, IDisposable
{
    IMPacketReader OpenMainStream();
}
