using FFmpeg.AutoGen;

namespace MFFAmpeg.AVFormats;

public unsafe class MInputFormat
{
    public AVInputFormat* Format { get { return _format; } }

    private readonly AVInputFormat* _format;

    public MInputFormat(string shortName)
    {
        _format = ffmpeg.av_find_input_format(shortName);
    }
}
