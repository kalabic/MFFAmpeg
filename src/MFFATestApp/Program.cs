using FFmpeg.AutoGen;
using MFFAmpeg;
using MFFAmpeg.AVBuffers;
using MFFAmpeg.AVFormats;
using MFFAmpeg.Base;

namespace MFFATestApp;


internal class Program
{
    private static readonly string OUTPUT_FILE = "audio_decoder_test.wav";

    private const int PCM_BUFFER_SIZE = 256 * 1024;

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
            MFFApi.OpenAudioReader(inputPath, null)
            .ThrowIfError();

        var inputStream =
            reader.OpenMainStream()
            .ThrowIfError();

        if (inputStream.Codec.CodecID == AVCodecID.AV_CODEC_ID_NONE)
        {
            Console.WriteLine("Unknown codec.");
        }
        else if (inputStream.Codec.CodecID < AVCodecID.AV_CODEC_ID_FIRST_AUDIO)
        {
            Console.WriteLine("Not an audio codec, unsupported or unknown");
        }
        else if (inputStream.Codec.CodecID == AVCodecID.AV_CODEC_ID_PCM_S16LE)
        {
            Console.WriteLine("Running PCM reader test...");
            RunPCMReader(inputStream);
        }
        else
        {
            Console.WriteLine("Running decoder test...");
            RunDecoder(inputStream, OUTPUT_FILE);
        }

        reader.Dispose();
    }

    private static void RunPCMReader(IMPacketReader inputStream)
    {
        int packetCount = 0;

        foreach (var packet in inputStream)
        {
            packetCount++;
            Console.Write($"\rRead packet #{packetCount}");
            packet.Dispose();
        }
    }

    private static void RunDecoder(IMPacketReader inputStream, string outputFile)
    {
        var decoder = 
            MFFApi.CreateDecoder(inputStream.Codec, inputStream.CodecParameters)
            .ThrowIfError();

        var decoderFormat = 
            decoder.AudioSampleFormat;

        var audioBuffer = 
            new PCM_S16_Buffer(PCM_BUFFER_SIZE);

        var audioBufferFormat = 
            new MAudioStreamFormat(audioBuffer.SampleFormat, decoderFormat.SampleRate, audioBuffer.BitsPerSample, decoderFormat.NumChannels);

        var writer = 
            MFFApi.OpenAudioWriter(outputFile, MFFApi.FILE_FORMAT_WAV)
            .ThrowIfError();

        writer.AddStream(audioBufferFormat)
            .ThrowIfError();

        var outputStream =
            writer.StartPacketWriter()
            .ThrowIfError();

        MFrame frame = MFFApi.AllocFrame();

        int packetCount = 0;

        foreach (var packet in inputStream)
        {
            int result = decoder.SendPacket(packet);
            packet.Dispose();

            while (result >= 0)
            {
                result = decoder.ReceiveFrame(frame);
                if (result == ffmpeg.AVERROR(ffmpeg.EAGAIN) || result == ffmpeg.AVERROR_EOF)
                {
                    break;
                }

                unsafe
                {
                    audioBuffer.ConvertAndAppend(decoderFormat.Format, frame.Ptr->data, decoderFormat.NumChannels, frame.SampleCount);
                }
            }

            if (result == ffmpeg.AVERROR(ffmpeg.EAGAIN))
            {
                outputStream.WritePacketFromData(audioBuffer);

                // In current implementation packet takes ownership of allocated memory and empties MByteBuffer object,
                // so it needs to be allocated again. This will likely change, it's WIP.
                audioBuffer.Dispose();
                audioBuffer = new PCM_S16_Buffer(PCM_BUFFER_SIZE);

                packetCount++;
                Console.Write($"\rDecoded packet #{packetCount}");

                if (packetCount == 1000)
                {
                    break;
                }

                continue;
            }

            if (result == ffmpeg.AVERROR_EOF)
            {
                break;
            }
        }

        Console.WriteLine("");

        decoder.Dispose();
        audioBuffer.Dispose();
        writer.Dispose();
    }
}
