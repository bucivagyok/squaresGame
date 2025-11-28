using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace squares.Persistence
{
    public class SquaresTable
    {
        #region Fields
        private int tableSize;
        private int[,] hLines; //vízszintes vonalak
        private int[,] vLines; //függőleges vonalak
        private int[,] squares; //négyzetek (0 - nincs berajzolva, {1, 2} - melyik játékosé}
        #endregion

        #region Properties
        public int Size { get { return tableSize; } }
        public int this[int x, int y, bool vertical] { get { return (vertical ? vLines[x, y] : hLines[x, y]); } }
        public bool IsFull
        {
            get
            {
                foreach (int value in hLines)
                {
                    if (value == 0)
                    {
                        return false;
                    }
                }
                foreach (int value in hLines)
                {
                    if (value == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool IsEmpty(int i, int j, bool vertical)
        {
            if (vertical)
            {
                return vLines[i, j] == 0;
            }
            else
            {
                return hLines[i, j] == 0;
            }
        }
        #endregion

        #region Constructors
        public SquaresTable(int n)
        {
            if (n < 3)
                throw new ArgumentOutOfRangeException(nameof(n), "The table size is less than 3");
            tableSize = n;
            vLines = new int[n - 1, n];
            hLines = new int[n, n - 1];
            squares = new int[n - 1, n - 1];
        }
        #endregion

        #region Public Methods
        public void DrawLine(int row, int col, bool vertical, int player)
        { 
            if (vertical)
            {
                vLines[row, col] = player;
            }
            else
            {
                hLines[row, col] = player;
            }
        }

        public void ClaimSquare(int row, int col, int player)
        {
            if (row < 0 || row > tableSize + 1)
                throw new ArgumentOutOfRangeException();
            if (col < 0 || row > tableSize + 1)
                throw new ArgumentOutOfRangeException();
            if (player < 0 || player > 2)
                throw new ArgumentOutOfRangeException();
            squares[row, col] = player;
        }

        public bool CheckSquare(int row, int col) //Visszaadja, hogy be lett-e rajzolva egy négyzet
        {
            if (row < 0 || row > tableSize + 1)
                throw new ArgumentOutOfRangeException();
            if (col < 0 || col > tableSize + 1)
                throw new ArgumentOutOfRangeException();
            if (hLines[row, col] != 0 && vLines[row, col] != 0 && hLines[row+1, col] != 0 && vLines[row, col+1] != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetSquare(int x, int y)
        {
            return squares[x, y];
        }
        #endregion
    }
}
