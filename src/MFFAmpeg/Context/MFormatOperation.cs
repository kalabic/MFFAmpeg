using MFFAmpeg.Base;

namespace MFFAmpeg.Context;


/// <summary> See comments for <see cref="IMFormatOperation"/> </summary>
internal abstract class MFormatOperation : DisposableBase, IMFormatOperation
{
    public MFormatContext Context { get { return _context; } }

    public int FFerror { get { return _fferror; } }

    public string FFerrorText { get { return _fferrorText; } }

    public bool IsCancelled { get { return _cancellation.IsCancellationRequested; } }



    protected readonly MFormatContext _context;

    protected int _fferror = 0;

    protected string _fferrorText = "";

    protected CancellationToken _cancellation;

    internal MFormatOperation(int fferror)
    {
        _context = new MFormatContext();
        _fferror = fferror;
        _fferrorText = MFFApi.av_strerror(fferror);
        _cancellation = default;
    }

    internal MFormatOperation(MFormatContext context, CancellationToken cancellation = default)
    {
        _context = context;
        _cancellation = cancellation;
    }
}
