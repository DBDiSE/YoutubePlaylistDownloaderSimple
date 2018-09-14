using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WV_Playlist_cmd
{
    public class ProgressBar : IProgress<double>, IDisposable
    {
        private const int MaxBars = 25;

        private double _progress;
        private int _barsDrawn;
        private int _barsOffset;

        public ProgressBar()
        {
            Initialize();
        }

        private void Initialize()
        {

        }

        private void DrawProgress()
        {
            // Draw bars
            //var bars = (int)Math.Floor(_progress * MaxBars);
            //for (var i = _barsDrawn; i < bars; i++)
            //{
            //    Console.SetCursorPosition(_barsOffset + i, Console.CursorTop);
            //    Console.Write('-');
            //}

            //_barsDrawn = bars;

            // Draw text
            Console.SetCursorPosition(_barsOffset, Console.CursorTop);
            Console.Write($"{_progress:P0}");
        }

        public void Report(double value)
        {
            _progress = value;
            DrawProgress();
        }

        public void Dispose()
        {
            Console.WriteLine();
        }
    }
}
