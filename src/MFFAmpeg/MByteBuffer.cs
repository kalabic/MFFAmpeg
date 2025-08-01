using FFmpeg.AutoGen;
using MFFAmpeg.Base;

namespace MFFAmpeg;


/// <summary>
/// Presumably safe wrapper for memory allocated by <see cref="ffmpeg.av_malloc(ulong)"/>.
/// Used to create new <see cref="MPacket"/> for writting to output.
/// </summary>
public unsafe class MByteBuffer : DisposableBase
{
    /// <summary> Size of allocated memory. </summary>
    public ulong BytesAllocated { get { return _bytes_allocated; } }


    /// <summary> Number of bytes actually used is equal or smaller than size of allocated memory. </summary>
    public ulong BytesUsed { get { return _bytes_used; } set { _bytes_used = value; } }


    /// <summary> Pointer to allocated memory. </summary>
    public byte* Data { get { return _ptr; } }


    //
    // Private implementation details begin below.
    //


    private byte* _ptr;

    private ulong _bytes_allocated = 0;

    private ulong _bytes_used = 0;

    public MByteBuffer(int size)
        : this((ulong)size)
    {
    }

    public MByteBuffer(ulong size)
    {
        _ptr = (byte *)ffmpeg.av_malloc(size);
        if (_ptr is not null)
        {
            _bytes_allocated = size;
        }
    }

    /// <summary>
    /// Indicate to this class it doesn't own allocated memory any more, used by <see cref="MPacket.MPacket(MByteBuffer)"/>.
    /// </summary>
    /// <param name="size"></param>
    public void ReleaseData()
    {
        _ptr = null;
        _bytes_allocated = 0;
        _bytes_used = 0;
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        void DisposeResources()
        {
            if (_ptr is not null)
            {
                ffmpeg.av_free(_ptr);
                _bytes_allocated = 0;
                _bytes_used = 0;
            }
        }

        if (disposing)
        {
            DisposeResources();
        }
        else
        {
#if DEBUG_UNDISPOSED
            if (_ptr is not null)
            {
                throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
            }
#else
            DisposeResources();
#endif
        }
        base.Dispose(disposing);
    }
}
