namespace MFFAmpeg.Base;


/// <summary> Helper base class for implementing the dispose pattern. </summary>
public abstract class DisposableBase : IDisposable
{
    public bool Disposed { get { return _disposed; } }

    private bool _disposed = false;

    ~DisposableBase()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Release managed resources: disposing = true, release unmanaged resources: disposing = false
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
#if DEBUG_UNDISPOSED
        if (!_disposed && !disposing)
        {
            throw new InvalidOperationException($"Undisposed: {this.GetType().Name}");
        }
#endif
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}
