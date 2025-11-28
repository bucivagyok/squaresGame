using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace squares.Persistence
{
    public class SquaresFileDataAccess : ISquaresDataAccess
    {
        private int[] _scores = new int[2];
        private int _player = 0;
        private int _drawnLines = 0;
        public int[] Scores { get { return new int[2] { _scores[0], _scores[1] }; } }
        public int Player { get { return _player; } }
        public int DrawnLines { get { return _drawnLines; } }
        public async Task<SquaresTable> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    _drawnLines = 0;
                    string line = await reader.ReadLineAsync() ?? string.Empty;
                    string[] data = line.Split(' ');
                    int size = int.Parse(data[0]);
                    _player = int.Parse(data[1]);
                    SquaresTable table = new SquaresTable(size);
                    
                    for (int i = 0; i < table.Size - 1; i++)
                    {
                        line = await reader.ReadLineAsync() ?? string.Empty;
                        data = line.Split(' ');

                        for (int j = 0; j < table.Size; j++)
                        {
                            if (data[j] != "0")
                            {
                                table.DrawLine(i, j, true, int.Parse(data[j]));
                                _drawnLines++;
                            }
                        }
                    }

                    for (int i = 0; i < table.Size; i++)
                    {
                        line = await reader.ReadLineAsync() ?? string.Empty;
                        data = line.Split(' ');

                        for (int j = 0; j < table.Size - 1; j++)
                        {
                            if (data[j] != "0")
                            {
                                table.DrawLine(i, j, false, int.Parse(data[j]));
                                _drawnLines++;
                            }
                        }
                    }

                    for (int i = 0; i < table.Size - 1; i++)
                    {
                        line = await reader.ReadLineAsync() ?? string.Empty;
                        data = line.Split(' ');

                        for (int j = 0; j < table.Size - 1; j++)
                        {
                            table.ClaimSquare(i, j, int.Parse(data[j]));
                            if (int.Parse(data[j]) != 0)
                                _scores[int.Parse(data[j]) - 1]++;
                        }
                    }
                    return table;
                }
            }
            catch
            {
                throw new SquaresDataException();
            }
        }

        public async Task SaveAsync(string path, SquaresTable table, int player)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(table.Size + " " + player);
                    await writer.WriteLineAsync();
                    for (int i = 0; i < table.Size-1; i++)
                    {
                        for (int j = 0; j < table.Size; j++)
                        {
                            await writer.WriteAsync(table[i, j, true] + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                    
                    for (int i = 0; i < table.Size; i++)
                    {
                        for (int j = 0; j < table.Size - 1; j++)
                        {
                            await writer.WriteAsync(table[i, j, false] + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                    
                    for (int i = 0; i < table.Size-1; i++)
                    {
                        for (int j = 0; j < table.Size-1; j++)
                        {
                            await writer.WriteAsync(table.GetSquare(i, j).ToString() + " ");
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new SquaresDataException();
            }
        }
    }
}
