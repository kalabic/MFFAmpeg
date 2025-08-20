using FFmpeg.AutoGen;

namespace MFFAmpeg.AVFormats;


public struct MAudioStreamFormat
{
    public AVSampleFormat Format;

    public int SampleRate;

    public int BitsPerSample;

    public int NumChannels;


    public MAudioStreamFormat(AVSampleFormat format, int sample_rate, int bits_per_sample, int num_channels)
    {
        Format = format;
        SampleRate = sample_rate;
        BitsPerSample = bits_per_sample;
        NumChannels = num_channels;
    }

    public readonly long ByteToSampleCount(long size)
    {
        switch(Format)
        {
            case AVSampleFormat.AV_SAMPLE_FMT_S16:      // signed 16 bits
            case AVSampleFormat.AV_SAMPLE_FMT_S16P:     // signed 16 bits, planar
                return 8 * size / (16L * NumChannels);

            case AVSampleFormat.AV_SAMPLE_FMT_FLT:      // float
            case AVSampleFormat.AV_SAMPLE_FMT_FLTP:     // float, planar
                return 8 * size / (32L * NumChannels);

            case AVSampleFormat.AV_SAMPLE_FMT_NONE:     // not initialized
            case AVSampleFormat.AV_SAMPLE_FMT_U8:       // unsigned 8 bits
            case AVSampleFormat.AV_SAMPLE_FMT_DBL:      // double
            case AVSampleFormat.AV_SAMPLE_FMT_S32:      // signed 32 bits
            case AVSampleFormat.AV_SAMPLE_FMT_U8P:      // unsigned 8 bits, planar
            case AVSampleFormat.AV_SAMPLE_FMT_S32P:     // signed 32 bits, planar
            case AVSampleFormat.AV_SAMPLE_FMT_DBLP:     // double, planar
            case AVSampleFormat.AV_SAMPLE_FMT_S64:      // signed 64 bits
            case AVSampleFormat.AV_SAMPLE_FMT_S64P:     // signed 64 bits, planar
            default:
                throw new NotImplementedException();
        }
    }
}
