using FFmpeg.AutoGen;

namespace MFFAmpeg;


/// <summary> Wrapper around <see cref="AVCodec"/> </summary>
public unsafe class MCodec
{
    public static implicit operator AVCodec*(MCodec codec) { return codec._codec; }

    /// <summary> See <see cref="AVCodecID"/> for more information. </summary>
    public AVCodecID CodecID { get { return (_codec is null) ? AVCodecID.AV_CODEC_ID_NONE : _codec->id; } }

    public AVCodec* Ptr { get { return _codec; } }


    private AVCodec* _codec;

    public MCodec()
    {
        _codec = null;
    }

    public MCodec(AVCodec* codec)
    {
        _codec = codec;
    }
}
