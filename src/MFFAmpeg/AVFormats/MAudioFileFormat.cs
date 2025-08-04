using FFmpeg.AutoGen;

namespace MFFAmpeg.AVFormats;


public struct MAudioFileFormat
{
    public string          Extension;

    public AVSampleFormat  Format;

    public AVCodecID       CodecId;


    public MAudioFileFormat()
    {
        Extension = "";
        Format = AVSampleFormat.AV_SAMPLE_FMT_NONE;
        CodecId = AVCodecID.AV_CODEC_ID_NONE;
    }

    public MAudioFileFormat(string extension, AVSampleFormat format, AVCodecID codec_id)
    {
        Extension = extension;
        Format = format;
        CodecId = codec_id;
    }

    public string CodecName()
    {
        return ffmpeg.avcodec_get_name(CodecId);
    }
}
