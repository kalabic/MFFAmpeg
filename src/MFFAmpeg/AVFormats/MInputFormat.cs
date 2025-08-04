using FFmpeg.AutoGen;
using MFFAmpeg.Base;

namespace MFFAmpeg.AVFormats;


/// <summary>
/// See <see cref="AVInputFormat"/> and <see cref="ffmpeg.av_find_input_format(string)"/> 
/// for more information.
/// </summary>
public unsafe class MInputFormat
{
    public static implicit operator AVInputFormat*(MInputFormat format) { return format._format; }

    public string Extensions { get { return _extensions; } }

    public bool IsNull { get { return _format == null; } }


    private AVInputFormat* _format = null;

    private string _extensions = "";

    public MInputFormat(string shortName)
    {
        _format = ffmpeg.av_find_input_format(shortName);
        if (_format is not null && _format->extensions is not null)
        {
            var ext = FFmpegHelper.ToString(_format->extensions);
            _extensions = (ext is null) ? "" : ext;
        }
    }
}
