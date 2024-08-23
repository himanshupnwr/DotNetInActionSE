using System.Reflection;

namespace SukoduSolver
{
    public interface IBoard
    {
        //C# indexer
        int this[int row, int column] { get; set; }

        int Size { get; }
        int GridSize { get; }
    }
    public class Solver
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        /*n-dimensional arrays vs.arrays of arrays(jagged arrays)

        A common way of providing a 2D matrix is to use an array of arrays: int[][] board, 
        which translates to an array of int arrays.This type is different from the 2D array used in listing 6.9: 
        int[,] board.

        The main difference between a 2D array and an array of arrays is that the latter provides no 
        guarantee that each array is the same length.This type of array is called jagged.
        
        A 2D array created with a statement like int[,] board = new int[9, 9];, 
        however, creates a 9x9 matrix with no extra steps. The same applies to arrays of other dimensions. */
        private readonly IBoard _board;
        public Solver(IBoard board)
        {
            _board = board;
        }
        public bool IsValid()
        {
            int size = _board.Size;
            var usedSet = new HashSet<int>();
            for (int row = 0; row < size; row++)
            {
                usedSet.Clear();
                for (int col = 0; col < size; col++)
                {
                    int num = _board[row, col];
                    if (num == 0)
                        continue;
                    if (usedSet.Contains(num))
                        return false;
                    usedSet.Add(num);
                }
            }

            for (int col = 0; col < size; col++)
            {
                usedSet.Clear();
                for (int row = 0; row < size; row++)
                {
                    int num = _board[row, col];
                    if (num == 0)
                        continue;
                    if (usedSet.Contains(num))
                        return false;
                    usedSet.Add(num);
                }
            }

            int sqrt = _board.GridSize;
            for (int grid = 0; grid < size; grid++)
            {
                usedSet.Clear();
                int startCol = (grid % sqrt) * sqrt;
                int startRow = (grid / sqrt) * sqrt;
                for (int cell = 0; cell < size; cell++)
                {
                    int col = startCol + (cell % sqrt);
                    int row = startRow + (cell / sqrt);
                    int num = _board[row, col];
                    if (num == 0)
                        continue;
                    if (usedSet.Contains(num))
                        return false;
                    usedSet.Add(num);
                }
            }

            return true;
        }
    }
}
