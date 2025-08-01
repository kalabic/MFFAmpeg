using FFmpeg.AutoGen;

namespace MFFAmpeg.AVFormats;

public struct MAudioFileFormat
{
    public string          _extension;
    public AVSampleFormat  _format;
    public AVCodecID       _codec_id;

    public MAudioFileFormat(string extension, AVSampleFormat format, AVCodecID codec_id)
    {
        _extension = extension;
        _format = format;
        _codec_id = codec_id;
    }

    public readonly string CodecName()
    {
        return ffmpeg.avcodec_get_name(_codec_id);
    }
}

public struct MAudioStreamFormat
{
    public int _sample_rate;
    public int _bits_per_sample;
    public int _num_channels;

    public MAudioStreamFormat(int sample_rate, int bits_per_sample, int num_channels)
    {
        _sample_rate = sample_rate;
        _bits_per_sample = bits_per_sample;
        _num_channels = num_channels;
    }

    public readonly ulong ByteToSampleCount(ulong size)
    {
        return 8 * size / ((ulong)_bits_per_sample * (ulong)_num_channels);
    }
}
