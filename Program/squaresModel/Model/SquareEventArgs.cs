using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace squares.Model
{
    public class SquareEventArgs : EventArgs
    {
        private int _stepsLeft;
        private int _p1Score;
        private int _p2Score;
        private int _playerUp;

        public int StepLeft { get { return _stepsLeft; } }
        public int P1 {  get { return _p1Score; } }
        public int P2 { get { return _p2Score; } }
        public int PlayerUp { get { return _playerUp; } }

        public SquareEventArgs(int stepsLeft, int p1Score, int p2Score, int playerUp)
        {
            _stepsLeft = stepsLeft;
            _p1Score = p1Score;
            _p2Score = p2Score;
            _playerUp = playerUp;
        }
    }
}
