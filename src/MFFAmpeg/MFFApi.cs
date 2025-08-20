using FFmpeg.AutoGen;
using MFFAmpeg.AVBuffers;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Internal;
using System.Runtime.InteropServices;

namespace MFFAmpeg;


/// <summary>
/// Minimalistic API wrapper for FFmpeg.AutoGen library.
/// </summary>
public class MFFApi
{
    /// <summary> Can be used by <see cref="OpenAudioReader(string, MInputFormat?, CancellationToken)"/> 
    /// to make it open only WAV files.</summary>
    public static readonly MInputFormat INPUT_FORMAT_WAV = new MInputFormat("wav");


    /// <summary> Default output file format. </summary>
    public static readonly MAudioFileFormat FILE_FORMAT_WAV =
        new MAudioFileFormat("wav", AVSampleFormat.AV_SAMPLE_FMT_S16, AVCodecID.AV_CODEC_ID_PCM_S16LE);


    /// <summary> Enumerator object that can iterate over all registered muxers. </summary>
    public static readonly IEnumerable<MOutputFormat> MUXER_LIST = new FFmpegMuxerList();


    /// <summary>
    /// Return a description of the AVERROR code. In case of failure the global variable errno is set to indicate the error.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static unsafe string av_strerror(int error)
    {
        var bufferSize = 1024;
        var buffer = stackalloc byte[bufferSize];
        ffmpeg.av_strerror(error, buffer, (ulong)bufferSize);
        var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
#pragma warning disable CS8603 // Possible null reference return.
        return message;
#pragma warning restore CS8603 // Possible null reference return.
    }


    /// <summary>
    /// Find directory with shared FFmpeg library binaries and bind FFmpeg.AutoGen library to it.
    /// <para>
    /// All arguments are optional and will use default procedure of looking for 'FFmpeg\bin\x64'
    /// directory starting in current folder and going up all the way to the root.
    /// </para>
    /// <list type = "bullet">
    ///   <item>userPaths - User provided list of paths to look for. Default is null.</item>
    ///   <item>useInstalledLibs - Look for common paths where shared libraries are installed, depending on the OS. Default is false.</item>
    ///   <item>consoleVerbosity - Console output verbosity. Default is 0, do not print anything.</item>
    /// </list>
    /// </summary>
    /// <param name="userPaths">User provided list of paths to look for. Optional.</param>
    /// <param name="useInstalledLibs">Look for common paths where shared libraries are installed, depending on the OS. Default is false.</param>
    /// <param name="consoleVerbosity">Console output verbosity. Default is 0, do not print anything.</param>
    /// <returns></returns>
    public static int Register(string[]? userPaths = null, bool useInstalledLibs = false, int consoleVerbosity = 0)
    {
        int result = FFmpegBinariesHelper.RegisterFFmpegBinaries(userPaths, useInstalledLibs, consoleVerbosity);
        if (result < 0)
        {
            return result;
        }

        if (consoleVerbosity >= 2)
        {
            Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
            Console.WriteLine("Running in {0}-bit mode.", Environment.Is64BitProcess ? "64" : "32");
        }
        if (consoleVerbosity >= 1)
        {
            Console.WriteLine($"FFmpeg version info: {ffmpeg.av_version_info()}");
            Console.WriteLine($"LIBAVFORMAT Version: {ffmpeg.LIBAVFORMAT_VERSION_MAJOR}.{ffmpeg.LIBAVFORMAT_VERSION_MINOR}");
            Console.WriteLine();
        }

        return result;
    }


    /// <summary>
    /// Allocate new data buffer for a packet to be written using <see cref="Write(MPacket)"/>
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static MByteBuffer AllocPacketBuffer(long size)
    {
        return new MByteBuffer(size);
    }


    /// <summary>
    /// Allocates a new frame object, used to receive decoded frames from a decoder.
    /// </summary>
    /// <returns></returns>
    public static MFrame AllocFrame()
    {
        return new MFrame();
    }


    /// <summary>
    /// Allocates an AVCodecContext, fills it based on the values from the supplied codec parameters,
    /// and initializes AVCodecContext to use the given AVCodec.
    /// </summary>
    /// <param name="codec"></param>
    /// <param name="parameters"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static IMPacketDecoder CreateDecoder(MCodec codec, MCodecParameters parameters, CancellationToken cancellation = default)
    {
        return MPacketDecoder.Create(codec, parameters, cancellation);
    }


    /// <summary>
    /// This basically loads whole file stream into memory.
    /// </summary>
    /// <param name="inputStream"></param>
    /// <returns></returns>
    public static IList<MPacket> CreatePacketList(IMPacketReader inputStream)
    {
        return MPacketList.Create(inputStream);
    }


    /// <summary>
    /// Iterate through all muxers and find first that supports given file format.
    ///  If none is found, returned <see cref="MOutputFormat.IsNull"/> will be true.
    /// </summary>
    /// <param name="fileFormat"></param>
    /// <returns></returns>
    public static unsafe MOutputFormat FindMuxerForFileFormat(MAudioFileFormat fileFormat)
    {
        // TODO: In fact not correct procedure, but good enough for now.
        // TODO: Return a list of matching muxers if more than one was found.
        MOutputFormat format = new(null);
        foreach (var muxerFormat in MFFApi.MUXER_LIST)
        {
            if (muxerFormat.AudioCodec == fileFormat.CodecId &&
                muxerFormat.Extensions is not null &&
                muxerFormat.Extensions.Equals(fileFormat.Extension))
            {
                format = muxerFormat;
                break;
            }
        }
        return format;
    }


    /// <summary>
    /// Open file for reading audio stream from. WIP: hard coded to accept only WAV.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="ifmt"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static IMAudioReader OpenAudioReader(string path, MInputFormat? ifmt, CancellationToken cancellation = default)
    {
        return MAudioReader.Open(path, ifmt, cancellation);
    }


    /// <summary>
    /// Create file for output audio stream. WIP: hard coded to accept only WAV.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileFormat"></param>
    /// <returns></returns>
    public static IMAudioWriter OpenAudioWriter(string path, MAudioFileFormat fileFormat, CancellationToken cancellation = default)
    {
        return MAudioWriter.Open(path, fileFormat, cancellation);
    }
}
