using DotBase.Core;

namespace MFFAmpeg.Context;


/// <summary> See comments for <see cref="IMFormatOperation"/> </summary>
internal abstract class MCodecOperation : DisposableBase, IMCodecOperation
{
    public MCodecContext Context { get { return _context; } }

    public int FFerror { get {  return _fferror; } }

    public bool IsCancelled { get { return _cancellation.IsCancellationRequested; } }



    protected readonly MCodecContext _context;

    protected int _fferror;

    protected CancellationToken _cancellation;

    internal MCodecOperation(int fferror)
    {
        _context = new MCodecContext();
        _fferror = fferror;
        _cancellation = default;
    }

    internal MCodecOperation(MCodecContext context, CancellationToken cancellation = default)
    {
        _context = context;
        _fferror = 0;
        _cancellation = cancellation;
    }
}
