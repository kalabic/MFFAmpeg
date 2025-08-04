using FFmpeg.AutoGen;
using MFFAmpeg.Base;

namespace MFFAmpeg.AVFormats;

/// <summary>
/// See <see cref="AVOutputFormat"/> for more information.
/// </summary>
public unsafe class MOutputFormat
{
    public static implicit operator AVOutputFormat*(MOutputFormat format) { return format._format; }

    public AVCodecID AudioCodec { get { return _format->audio_codec; } }

    public string Extensions { get { return _extensions; } }

    public bool IsNull { get { return _format == null; } }


    private AVOutputFormat* _format = null;

    private string _extensions = "";

    public MOutputFormat(AVOutputFormat* format)
    {
        _format = format;
        if (format is not null && format->extensions is not null)
        {
            var ext = FFmpegHelper.ToString(format->extensions);
            _extensions = (ext is null) ? "" : ext;
        }
    }
}
