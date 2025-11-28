using System.Drawing;
using squares.Persistence;

namespace squares.Model
{
    public class SquareGameModel
    {
        #region Fields
        private ISquaresDataAccess _dataAccess;
        private SquaresTable _table = null!;
        private int _player;
        private int _emptyLines;
        private int[] _scores = null!;
        #endregion

        #region Properties
        public SquaresTable Table { get { return _table; } }
        public int Player { get { return _player; } }
        public int EmptyLines { get { return _emptyLines; } }
        public int[] Scores { get { return new int[2] { _scores[0], _scores[1] }; } }
        #endregion

        #region Events
        public event EventHandler<SquareFieldEventArgs>? FieldChanged;
        public event EventHandler<SquareFieldEventArgs>? SquareClaimed;
        public event EventHandler<SquareEventArgs>? GameAdvanced;
        public event EventHandler<SquareEventArgs>? GameOver;
        public event EventHandler<SquareEventArgs>? GameCreated;
        #endregion

        #region Constructor
        public SquareGameModel(ISquaresDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            NewGame(3);
        }
        #endregion

        #region Public game methods
        public void NewGame(int n)
        {
            _table = new SquaresTable(n);
            _emptyLines = 2 * n * (n - 1);
            _player = 1;
            _scores = new int[] { 0, 0 };
            OnGameCreated();
            //OnGameAdvanced(_emptyLines, 0, 0, _player);
        }

        public void Step(int row, int col, bool vertical)
        {
            _table.DrawLine(row, col, vertical, _player);
            OnFieldChanged(row, col, vertical, _player);

            bool claimed = false;

            if (vertical)
            {
                if (col != 0)
                {
                    if (_table.CheckSquare(row, col - 1))
                    {
                        _table.ClaimSquare(row, col - 1, _player);
                        claimed = true;
                        _scores[_player-1]++;
                        OnSquareClaimed(row, col - 1, _player);
                    }
                }
                if (col != _table.Size-1)
                {
                    if (_table.CheckSquare(row, col))
                    {
                        _table.ClaimSquare(row, col, _player);
                        claimed = true;
                        _scores[_player - 1]++;
                        OnSquareClaimed(row, col, _player);
                    }
                }
            }
            else
            {
                if (row != 0)
                {
                    if (_table.CheckSquare(row - 1, col))
                    {
                        _table.ClaimSquare(row - 1, col, _player);
                        claimed = true;
                        _scores[_player - 1]++;
                        OnSquareClaimed(row - 1, col, _player);
                    }
                }
                if (row != _table.Size-1)
                {
                    if (_table.CheckSquare(row, col))
                    {
                        _table.ClaimSquare(row, col, _player);
                        claimed = true;
                        _scores[_player - 1]++;
                        OnSquareClaimed(row, col, _player);
                    }
                }
            }

            _emptyLines--;
            if (_emptyLines == 0)
            {
                OnGameOver(_scores[0], _scores[1]);
            }

            if (!claimed)
            {
                _player = 1 + (_player % 2);
            }
            OnGameAdvanced(_emptyLines, _scores[0], _scores[1], _player);
        }

        public async Task LoadGameAsync(string path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided");

            _table = await _dataAccess.LoadAsync(path);
            _player = _dataAccess.Player;
            _scores = _dataAccess.Scores;
            _emptyLines = (2 * Table.Size * (Table.Size - 1)) - _dataAccess.DrawnLines;

            OnGameCreated();
        }

        public async Task SaveGameAsync(string path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided");

            await _dataAccess.SaveAsync(path, _table, _player);
        }

        public void OnGameLoaded()
        {
            OnGameCreated();
        }
        #endregion

        #region Private event methods
        private void OnFieldChanged(int x, int y, bool vertical, int player)
        {
            FieldChanged?.Invoke(this, new SquareFieldEventArgs(x, y, vertical, player));
        }

        private void OnSquareClaimed(int x, int y, int player)
        {
            SquareClaimed?.Invoke(this, new SquareFieldEventArgs(x, y, player));
        }

        private void OnGameAdvanced(int steps, int p1, int p2, int player)
        {
            GameAdvanced?.Invoke(this, new SquareEventArgs(steps, p1, p2, player));
        }

        private void OnGameOver(int p1, int p2)
        {
            GameOver?.Invoke(this, new SquareEventArgs(0, p1, p2, 0));
        }

        private void OnGameCreated()
        {
            GameCreated?.Invoke(this, new SquareEventArgs(_emptyLines, _scores[0], _scores[1], _player));
        }

        #endregion
    }
}
