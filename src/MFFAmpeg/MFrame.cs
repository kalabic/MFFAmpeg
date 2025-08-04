using FFmpeg.AutoGen;

namespace MFFAmpeg;

/// <summary>
/// Wrapper around <see cref="AVFrame"/>. It is a structure that describes decoded (raw) audio or video data.
/// </summary>
public unsafe class MFrame
{
    public static implicit operator AVFrame*(MFrame frame) { return frame._frame; }

    public AVFrame* Ptr { get { return _frame; } }

    public int SampleCount { get { return _frame->nb_samples; } }


    private AVFrame* _frame = null;

    internal MFrame()
    {
        _frame = ffmpeg.av_frame_alloc();
    }
}
