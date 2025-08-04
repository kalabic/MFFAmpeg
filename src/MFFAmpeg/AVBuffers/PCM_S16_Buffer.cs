using FFmpeg.AutoGen;

namespace MFFAmpeg.AVBuffers;


public class PCM_S16_Buffer : MByteBuffer
{
    private const float CONVERT_FACTOR_SHORT = 32768.0f;

    public AVSampleFormat SampleFormat { get {  return AVSampleFormat.AV_SAMPLE_FMT_S16; } }

    public int BitsPerSample { get { return 16; } }


    public PCM_S16_Buffer(int size) : base(size) { }

    public PCM_S16_Buffer(ulong size) : base(size) { }

    public unsafe void ConvertAndAppend(AVSampleFormat format, byte_ptrArray8 channels, int numChannels, int numSamples)
    {
        if (format == AVSampleFormat.AV_SAMPLE_FMT_FLTP)
        {
            float*[] src = new float*[numChannels];
            for (int i = 0; i < numChannels; i++)
            {
                src[i] = (float*)channels[(uint)i];
            }

            short* dst = (short*)(_ptr + _bytes_used);

            for (int i = 0; i < numSamples; i++)
            {
                for (uint ch = 0; ch < numChannels; ch++)
                {
                    *dst++ = (short)(src[ch][i] * CONVERT_FACTOR_SHORT);
                }
            }

            int outputBytes = numSamples * numChannels * 2;
            _bytes_used += (ulong)outputBytes;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
