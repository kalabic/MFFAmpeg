namespace MFFAmpeg.Internal;

internal class MTimestamp : IMTimestamp
{
    public long Timestamp { get { return _timestamp; } }

    public long Duration { get { return _duration; } }

    internal long _timestamp = 0;

    internal long _duration = 0;
}
