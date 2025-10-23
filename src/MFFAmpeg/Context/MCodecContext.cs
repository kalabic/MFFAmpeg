using DotBase.Core;
using FFmpeg.AutoGen;

namespace MFFAmpeg.Context;


public unsafe class MCodecContext : FinalizableBase
{
    public static unsafe MCodecContext? Create(MCodec codec, MCodecParameters parameters)
    {
        AVCodecContext* context = ffmpeg.avcodec_alloc_context3(codec);
        if (context is null)
        {
            return null;
        }

        int fferror = 0;
        fferror = ffmpeg.avcodec_parameters_to_context(context, parameters);
        if (fferror < 0)
        {
            ffmpeg.avcodec_free_context(&context);
            return null;
        }

        fferror = ffmpeg.avcodec_open2(context, codec, null);
        if (fferror < 0)
        {
            ffmpeg.avcodec_free_context(&context);
            return null;
        }

        return new MCodecContext(context);
    }

    public static implicit operator AVCodecContext*(MCodecContext context) { return context._context; }

    public static implicit operator AVCodecContext**(MCodecContext context) { return context.PPtr; }

    public AVCodecContext* Ptr { get { return _context; } }

    public AVCodecContext** PPtr 
    { 
        get 
        { 
            fixed (AVCodecContext** pptr = &_context) 
            { 
                return pptr;
            }
        } 
    }

    public bool IsNotNull { get { return _context != null; } }

    public bool IsNull { get { return _context == null; } }

    public bool IsValid { get { return _context != null; } }

    public bool IsNotValid { get { return _context == null; } }


    private AVCodecContext* _context = null;

    internal unsafe MCodecContext()
    {
        _context = null;
    }

    internal unsafe MCodecContext(AVCodecContext* context)
    {
        _context = context;
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        void DisposeResources()
        {
            if (_context is not null)
            {
                unsafe
                {
                    fixed (AVCodecContext** ppContext = &_context)
                    {
                        ffmpeg.avcodec_free_context(ppContext);
                        _context = null;
                    }
                }
            }
        }

        if (disposing)
        {
            DisposeResources();
        }
        else
        {
            if (_context is not null)
            {
#if DEBUG_UNDISPOSED
                throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
#else
                DisposeResources();
#endif
            }
        }
        base.Dispose(disposing);
    }
}
