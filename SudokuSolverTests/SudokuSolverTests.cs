using Newtonsoft.Json.Linq;
using SukoduSolver;
using System.Drawing;

namespace SudokuSolverTests
{
    //Fact tests don’t take inputs and usually test one thing.
    //Theory tests take inputs, and each input set is considered to be a separate test.
    //InlineData is an attribute that can pass constant values to a Theory.
    //MemberData points to a static member to get Theory inputs.

    public class SudokuSolverTests
    {
        [Fact]
        public void Empty4x4Board()
        {
            IBoard board = new ArrayBoard(4);
            int[,] empty = new int[4, 4];
            var solver = new Solver(board);
            Assert.True(solver.IsValid());
            //Assert.True(solver.IsValid(), "IsValid check");)
        }
        [Fact]
        public void NonSquareBoard()
        {
            IBoard board = new ArrayBoard(4);
            int[,] empty = new int[4, 9];
            var solver = new Solver(board);
            Assert.False(solver.IsValid());
        }
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(4, true)]
        [InlineData(8, false)]
        [InlineData(9, true)]
        [InlineData(10, false)]
        [InlineData(16, true)]
        public void EmptyBoardSizes1(int size, bool isValid)
        {
            IBoard board = new ArrayBoard(size);
            int[,] empty = new int[size, size];
            var solver = new Solver(board);
            Assert.Equal(isValid, solver.IsValid());
        }

        [Theory]
        [InlineData(8, false)]
        [InlineData(9, true)]
        public void EmptyBoardSizes2(int size, bool isValid)
        {
            if (!isValid)
            {
                Assert.Throws<ArgumentException>("size", () => new ArrayBoard(size));
            }
            else
            {
                _ = new ArrayBoard(size);
            }
        }

        //MemberData is another attribute that we can use with a Theory test.
        //This attribute tells xUnit to use a member to get the parameters to pass to the test
        [Theory]
        [MemberData(nameof(Boards))]
        public void CheckRules(IBoard board, bool isValid)
        {
            var solver = new Solver(board);
            Assert.Equal(isValid, solver.IsValid());
        }

        //yield returns - To return an enumeration of parameters to send to the theory,
        //we could build an array or List or use yield return.
        //The array/List option creates an extra object to hold all the values.
        //When xUnit goes to get the value of Boards,
        //it gets the enumerator from the IEnumerable and enumerates through to the end.
        //yield return means that we don’t care what creates the enumeration and want to
        //return the values one at a time.
        public static IEnumerable<object[]> Boards
        {
            get
            {
                IBoard board = new ArrayBoard(4);
                board[1, 0] = 1;
                board[3, 0] = 1;
                yield return new object[] { board, false };
                board = new ArrayBoard(4);
                board[1, 0] = 1;
                board[1, 2] = 1;
                yield return new object[] { board, false };
                board = new ArrayBoard(4);
                board[1, 2] = 1;
                board[0, 3] = 1;
                yield return new object[] { board, false };
                board = new ArrayBoard(4);
                board[1, 1] = 1;
                board[2, 3] = 1;
                yield return new object[] { board, true };
            }
        }
    }
}