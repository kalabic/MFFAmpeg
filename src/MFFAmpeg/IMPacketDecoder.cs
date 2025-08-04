using MFFAmpeg.AVFormats;

namespace MFFAmpeg;


/// <summary>
/// Interface is following 'protocol' for using FFmpeg audio decoding API that works by
/// first sending packet to decoder and then reading decoded frames one by one until
/// <see cref="ReceiveFrame(MFFAmpeg.MFrame)"/> returns ffmpeg.EAGAIN
/// </summary>
public interface IMPacketDecoder : IMCodecOperation
{
    /// <summary> Sample format of decoded audio received in frames returned by <see cref="ReceiveFrame(MFrame)"/> </summary>
    MAudioStreamFormat AudioSampleFormat { get; }


    /// <summary>
    /// <para>
    /// According to documentation in 'avcodec.h' header file:
    /// </para>
    /// <list type = "bullet">
    ///   <item>0 - Success.</item>
    ///   <item>AVERROR(EAGAIN) - Input is not accepted in the current state - user must read output with avcodec_receive_frame() (once all output is read, the packet should be resent, and the call will not fail with EAGAIN).</item>
    ///   <item>AVERROR_EOF - The decoder has been flushed, and no new packets can be sent to it.</item>
    ///   <item>AVERROR(EINVAL) - Codec not opened, it is an encoder, or requires flush.</item>
    ///   <item>AVERROR(ENOMEM) - Failed to add packet to internal queue, or similar.</item>
    ///   <item>"another negative error code" - Legitimate decoding errors.</item>
    /// </list>
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    int SendPacket(MPacket packet);


    /// <summary>
    /// <para>
    /// According to documentation in 'avcodec.h' header file:
    /// </para>
    /// <list type = "bullet">
    ///   <item>0 - Success, a frame was returned.</item>
    ///   <item>AVERROR(EAGAIN) - Output is not available in this state - user must try to send new input.</item>
    ///   <item>AVERROR_EOF - The codec has been fully flushed, and there will be no more output frames.</item>
    ///   <item>AVERROR(EINVAL) - Codec not opened, or it is an encoder without the AV_CODEC_FLAG_RECON_FRAME flag enabled.</item>
    ///   <item>"other negative error code" Legitimate decoding errors.</item>
    /// </list>
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    int ReceiveFrame(MFrame frame);
}
