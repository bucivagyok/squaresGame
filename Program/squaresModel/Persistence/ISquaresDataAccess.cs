using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace squares.Persistence
{
    public interface ISquaresDataAccess
    {
        public int[] Scores { get; }
        public int Player { get; }
        public int DrawnLines { get; }
        Task<SquaresTable> LoadAsync(string path);

        Task SaveAsync(string path, SquaresTable table, int player);
    }
}
