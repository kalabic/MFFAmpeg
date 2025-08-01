using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Internal;
using System.Runtime.InteropServices;

namespace MFFAmpeg;


/// <summary>
/// Minimalistic API wrapper for FFmpeg.AutoGen library.
/// </summary>
public class MFFApi
{
    /// <summary> Default input file format. </summary>
    public static readonly MInputFormat INPUT_FORMAT_WAV = new MInputFormat("wav");


    /// <summary> Default audio stream format. </summary>
    public static readonly MAudioFileFormat DEFAULT_AUDIO_FILE_FORMAT =
        new MAudioFileFormat("wav", AVSampleFormat.AV_SAMPLE_FMT_S16, AVCodecID.AV_CODEC_ID_PCM_S16LE);


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
