using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;

namespace MFFAmpeg.Context;


public unsafe class MFormatContext : IDisposable
{
    public static unsafe MFormatContext? Create()
    {
        AVFormatContext* context = ffmpeg.avformat_alloc_context();
        if (context is null)
        {
            return null;
        }

        return new MFormatContext(context);
    }


    public static unsafe MFormatContext? CreateOutput(string path, MOutputFormat format, ref int fferror)
    {
        MFormatContext context = new MFormatContext();
        fferror = ffmpeg.avformat_alloc_output_context2(context, format, "", path);
        if (fferror < 0)
        {
            return null;
        }

        AVIOContext* ioContext;
        fferror = ffmpeg.avio_open2(&ioContext, path, ffmpeg.AVIO_FLAG_WRITE, null, null);
        if (fferror < 0)
        {
            ffmpeg.avformat_free_context(context);
            return null;
        }
        context._context->pb = ioContext;

        return new MFormatContext(context);
    }

    public static implicit operator AVFormatContext*(MFormatContext context) { return context._context; }

    public static implicit operator AVFormatContext**(MFormatContext context) { return context.PPtr; }

    public AVFormatContext* Ptr { get { return _context; } }

    public AVFormatContext** PPtr 
    { 
        get 
        { 
            fixed (AVFormatContext** pptr = &_context) 
            { 
                return pptr;
            }
        } 
    }

    public bool IsValid { get { return _context != null; } }

    public bool IsNotValid { get { return _context == null; } }


    private AVFormatContext* _context = null;

    internal unsafe MFormatContext()
    {
        _context = null;
    }

    internal unsafe MFormatContext(AVFormatContext* context)
    {
        _context = context;
    }

    public void Dispose()
    {
        if (_context is not null)
        {
            fixed (AVFormatContext** ppContext = &_context)
            {
                if (_context->pb is not null)
                {
                    ffmpeg.avformat_close_input(ppContext);
                }
                else
                {
                    ffmpeg.avformat_free_context(_context);
                }
            }
            _context = null;
        }
    }
}
