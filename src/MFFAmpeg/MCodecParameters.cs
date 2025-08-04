using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;

namespace MFFAmpeg;


/// <summary> Wrapper around <see cref="AVCodecParameters"/> </summary>
public unsafe class MCodecParameters
{
    public static implicit operator AVCodecParameters*(MCodecParameters codec) { return codec._parameters; }

    public MAudioStreamFormat StreamFormat 
    {
        get
        {
            return new MAudioStreamFormat(
                (AVSampleFormat)_parameters->format, 
                _parameters->sample_rate, 
                _parameters->bits_per_coded_sample, 
                _parameters->ch_layout.nb_channels);
        }
    }

    public AVCodecParameters* Ptr { get { return _parameters; } }


    private AVCodecParameters* _parameters;

    public MCodecParameters()
    {
        _parameters = null;
    }

    public MCodecParameters(AVCodecParameters* parameters)
    {
        _parameters = parameters;
    }
}
