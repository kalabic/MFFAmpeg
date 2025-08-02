using MFFAmpeg;
using MFFAmpeg.Base;

namespace MFFATestApp;

internal class Program
{
    /// <summary>
    /// 
    /// The Main
    /// 
    /// </summary>
    /// <param name="args">Input audio file path.</param>
    private static void Main(string[] args)
    {
        if (MFFApi.Register() < 0)
        {
            Console.WriteLine($"FFmpeg shared libraries not found. Cannot continue.");
            return;
        }

        if (args.Length != 1)
        {
            Console.WriteLine($"A single argument is required, path to input audio file.");
            return;
        }
        Console.WriteLine($"Input file path: {args[0]}");


        RunTest(args[0]);
    }

    private static void RunTest(string inputPath)
    {
        var reader = 
            MFFApi.OpenAudioReader(inputPath, MFFApi.INPUT_FORMAT_WAV)
            .ThrowIfError();

        var inputStream = 
            reader.OpenMainStream()
            .ThrowIfError();

        var list = 
            MFFApi.CreatePacketList(inputStream);

        Console.WriteLine($"{list.Count} packets loaded from main audio stream.");

        reader.Dispose();
    }
}
