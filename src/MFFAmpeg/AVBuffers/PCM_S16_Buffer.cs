using AudioFormatLib;
using AudioFormatLib.Extensions;
using FFmpeg.AutoGen;

namespace MFFAmpeg.AVBuffers;


public unsafe class PCM_S16_Buffer : MByteBuffer
{
    public AVSampleFormat SampleFormat { get {  return AVSampleFormat.AV_SAMPLE_FMT_S16; } }

    public int BitsPerSample { get { return 16; } }


    public PCM_S16_Buffer(int size) : this((long)size) { }

    public PCM_S16_Buffer(long size) : base(size) { }

    public unsafe void ConvertAndAppend(AVSampleFormat format, byte_ptrArray8 channels, int numChannels, int numSamples)
    {
        if (format == AVSampleFormat.AV_SAMPLE_FMT_FLTP && numChannels == 2)
        {
            // Rule is that when spans are created, their size is measured as a total number of samples accross all channels.
            var output = ((Ptr_t<short>)(_ptr + _bytes_used)).AsSampleSpan(numSamples * numChannels, numChannels);
            var leftInput = ((Ptr_t<float>)channels[0]).AsSampleSpan(numSamples);
            var rightInput = ((Ptr_t<float>)channels[1]).AsSampleSpan(numSamples);

            ATools.ConvertToStereo(leftInput, rightInput, output);

            int outputBytes = numSamples * numChannels * sizeof(short);
            _bytes_used += outputBytes;
        }
        else if (format == AVSampleFormat.AV_SAMPLE_FMT_FLTP && numChannels == 1)
        {
            var output = ((Ptr_t<short>)(_ptr + _bytes_used)).AsSampleSpan(numSamples * numChannels, numChannels);
            var monoTrack = ((Ptr_t<float>)channels[0]).AsSampleSpan(numSamples);

            ATools.Convert(monoTrack, output);

            int outputBytes = numSamples * numChannels * sizeof(short);
            _bytes_used += outputBytes;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
