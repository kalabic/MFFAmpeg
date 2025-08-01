using FFmpeg.AutoGen;
using System.Runtime.InteropServices;

namespace MFFAmpeg.Internal;

internal static class FFmpegBinariesHelper
{
    internal static int RegisterFFmpegBinaries(string[]? userPaths, bool useInstalledLibs, int consoleVerbosity = 0)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (userPaths is not null)
            {
                for (int i=0; i<userPaths.Length; i++)
                {
                    if (Directory.Exists(userPaths[i]))
                    {
                        if (consoleVerbosity >= 2)
                        {
                            Console.WriteLine($"FFmpeg binaries found in: {userPaths[i]}");
                        }

                        ffmpeg.RootPath = userPaths[i];
                        return 0;
                    }
                }
            }
            else if (useInstalledLibs)
            {
                var chocoFFmpegPath = "C:\\ProgramData\\chocolatey\\lib\\ffmpeg-shared\\tools\\ffmpeg-7.1.1-full_build-shared\\bin";
                if (Directory.Exists(chocoFFmpegPath))
                {
                    if (consoleVerbosity >= 2)
                    {
                        Console.WriteLine($"FFmpeg binaries found in: {chocoFFmpegPath}");
                    }
                    ffmpeg.RootPath = chocoFFmpegPath;
                    return 0;
                }
            }
            else
            {
                var current = Environment.CurrentDirectory;
                var probe = Path.Combine("FFmpeg", "bin", Environment.Is64BitProcess ? "x64" : "x86");

                while (current != null)
                {
                    var ffmpegBinaryPath = Path.Combine(current, probe);

                    if (Directory.Exists(ffmpegBinaryPath))
                    {
                        if (consoleVerbosity >= 2)
                        {
                            Console.WriteLine($"FFmpeg binaries found in: {ffmpegBinaryPath}");
                        }
                        ffmpeg.RootPath = ffmpegBinaryPath;
                        return 0;
                    }

                    current = Directory.GetParent(current)?.FullName;
                }
            }
        }
        else
        {
            throw new NotSupportedException(); // fell free add support for platform of your choose
        }

        return -1;
    }
}
