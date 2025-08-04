using FFmpeg.AutoGen;
using MFFAmpeg.AVFormats;
using System.Collections;

namespace MFFAmpeg.Internal;


/// <summary> C# friendly wrapper around FFmpeg's muxer iterator. See <see cref="ffmpeg.av_muxer_iterate"/> </summary>
internal class FFmpegMuxerList : IEnumerable<MOutputFormat>
{
    internal unsafe class FFmpegMuxerEnumerator : IEnumerator<MOutputFormat>
    {
        public MOutputFormat Current { get { return _current; } }


        private void* _FFmpegIt = null;

        private MOutputFormat _current = new(null);

        object IEnumerator.Current => Current;

        public void Dispose()
        { }

        public bool MoveNext()
        {
            fixed (void** ppFFmpegIt = &_FFmpegIt)
            {
                _current = new MOutputFormat(ffmpeg.av_muxer_iterate(ppFFmpegIt));
            }
            return !_current.IsNull;
        }

        public void Reset()
        {
            _FFmpegIt = null;
        }
    }


    public IEnumerator<MOutputFormat> GetEnumerator()
    {
        return new FFmpegMuxerEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
