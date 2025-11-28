using squares.Model;
using squares.Persistence;
using System.Runtime.CompilerServices;
using Moq;

namespace Squares.Test
{
    [TestClass]
    public class SquareGameModelTest
    {
        private SquareGameModel _model = null!;
        private SquaresTable _mockedTable = null!;
        private Mock<ISquaresDataAccess> _mock = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new SquaresTable(3);
            _mockedTable.DrawLine(1, 2, true, 1);
            _mockedTable.DrawLine(0, 2, true, 2);
            _mockedTable.DrawLine(2, 1, false, 1);

            _mock = new Mock<ISquaresDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(_mockedTable));

            _model = new SquareGameModel(_mock.Object);

            _model.GameAdvanced += new EventHandler<SquareEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<SquareEventArgs>(Model_GameOver);
        }

        [TestMethod]
        public void SquaresGameModelNewGame3Test()
        {
            _model.NewGame(3);

            Assert.AreEqual(12, _model.EmptyLines);
            Assert.AreEqual(3, _model.Table.Size);

            int notEmpty = 0;
            for (int i = 0; i < _model.Table.Size - 1; i++)
                for (int j = 0; j < _model.Table.Size; j++)
                    if (!_model.Table.IsEmpty(i, j, true))
                        notEmpty++;

            for (int i = 0; i < _model.Table.Size; i++)
                for (int j = 0; j < _model.Table.Size - 1; j++)
                    if (!_model.Table.IsEmpty(i, j, false))
                        notEmpty++;

            Assert.AreEqual(0, notEmpty);
        }

        [TestMethod]
        public void SquaresGameModelNewGame5Test()
        {
            _model.NewGame(5);

            Assert.AreEqual(40, _model.EmptyLines);
            Assert.AreEqual(5, _model.Table.Size);

            int notEmpty = 0;
            for (int i = 0; i < _model.Table.Size - 1; i++)
                for (int j = 0; j < _model.Table.Size; j++)
                    if (!_model.Table.IsEmpty(i, j, true))
                        notEmpty++;

            for (int i = 0; i < _model.Table.Size; i++)
                for (int j = 0; j < _model.Table.Size - 1; j++)
                    if (!_model.Table.IsEmpty(i, j, false))
                        notEmpty++;

            Assert.AreEqual(0, notEmpty);
        }

        [TestMethod]
        public void SquaresGameModelNewGame9Test()
        {
            _model.NewGame(9);

            Assert.AreEqual(144, _model.EmptyLines);
            Assert.AreEqual(9, _model.Table.Size);

            int notEmpty = 0;
            for (int i = 0; i < _model.Table.Size - 1; i++)
                for (int j = 0; j < _model.Table.Size; j++)
                    if (!_model.Table.IsEmpty(i, j, true))
                        notEmpty++;

            for (int i = 0; i < _model.Table.Size; i++)
                for (int j = 0; j < _model.Table.Size - 1; j++)
                    if (!_model.Table.IsEmpty(i, j, false))
                        notEmpty++;

            Assert.AreEqual(0, notEmpty);
        }

        [TestMethod]
        public void SquaresGameModelStepTest()
        {
            Assert.AreEqual(12, _model.EmptyLines);

            _model.Step(0, 0, false);
            _model.Step(1, 0, false);
            _model.Step(0, 0, true);
            _model.Step(0, 1, true);

            Assert.AreEqual(8, _model.EmptyLines);
            Assert.AreEqual(1, _model.Scores[1]);
            Assert.AreEqual(2, _model.Player);
            Assert.AreEqual(2, _model.Table.GetSquare(0, 0));
        }

        [TestMethod]
        public async Task SquaresGameModelLoadTest()
        {
            await _model.LoadGameAsync(string.Empty);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 2; j++)
                {
                    Assert.AreEqual(_mockedTable[i, j, false], _model.Table[i, j, false]);
                    Assert.AreEqual(_mockedTable[j, i, true], _model.Table[j, i, true]);
                }

            _mock.Verify(dataAccess => dataAccess.LoadAsync(string.Empty), Times.Once());
        }

        private void Model_GameAdvanced(object? sender,  SquareEventArgs e)
        {
            Assert.AreEqual(e.StepLeft, _model.EmptyLines);
            Assert.AreNotEqual(0, _model.EmptyLines);
        }

        private void Model_GameOver(object? sender, SquareEventArgs e)
        {
            Assert.AreEqual(0, _model.EmptyLines);
        }
    }
}