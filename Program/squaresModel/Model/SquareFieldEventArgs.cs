using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace squares.Model
{
    public class SquareFieldEventArgs : EventArgs
    {
        private int _changedFieldX;
        private int _changedFieldY;
        private bool? _changedVertical;
        private int _player;

        public int X { get { return _changedFieldX; } }
        public int Y { get { return _changedFieldY; } }
        public bool? Vertical { get { return _changedVertical; } }
        public int Player { get { return _player; } }

        public SquareFieldEventArgs(int x, int y, bool vertical, int player)
        {
            _changedFieldX = x;
            _changedFieldY = y;
            _changedVertical = vertical;
            _player = player;
        }

        public SquareFieldEventArgs(int x, int y, int player)
        {
            _changedFieldX = x;
            _changedFieldY = y;
            _player = player;
        }
    }
}
