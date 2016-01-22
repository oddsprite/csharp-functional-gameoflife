using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace GameOfLife.Tests
{
    [TestFixture]
    public class GameOfLifeTests
    {
        [Test, TestCaseSource(nameof(IsNextToTests))]
        public void OnGridTest(Cell otherCell, IResolveConstraint expectation)
        {
            var cell = new Cell(3, 4, true);
            Assert.That(cell.IsNextTo(otherCell), expectation);
        }

        private static IEnumerable<TestCaseData> IsNextToTests()
        {
            yield return new TestCaseData(new Cell(1, 1, false), Is.False).SetName("IsNextTo_NonNeighbour_False");
            yield return new TestCaseData(new Cell(3, 4, false), Is.False).SetName("IsNextTo_Self_False");
            yield return new TestCaseData(new Cell(1, 4, false), Is.False).SetName("IsNextTo_SameLine_False");
            yield return new TestCaseData(new Cell(2, 2, false), Is.False).SetName("IsNextTo_KnightMoveAway_False");

            yield return new TestCaseData(new Cell(3, 3, false), Is.True).SetName("IsNextTo_Right_True");
            yield return new TestCaseData(new Cell(3, 5, false), Is.True).SetName("IsNextTo_Left_True");
            yield return new TestCaseData(new Cell(2, 4, false), Is.True).SetName("IsNextTo_Above_True");
            yield return new TestCaseData(new Cell(4, 4, false), Is.True).SetName("IsNextTo_Below_True");
            yield return new TestCaseData(new Cell(2, 3, false), Is.True).SetName("IsNextTo_Diagonal_True");
        }

        [Test]
        public void Run_With3Iterations_AllIterationsRun()
        {
            var grid = GameOfLife.GetGrid(new bool[1, 1]);
            int iterations = 3;
            int actualIterations = 0;

            Func<IEnumerable<Cell>, IEnumerable<Cell>> iterator = iterat =>
            {
                actualIterations++;
                return new List<Cell> { new Cell(1, 1, false) };
            };

            GameOfLife.Run(grid, iterations, iterator, (bools, i) => { }, () => { });

            Assert.That(actualIterations, Is.EqualTo(iterations));
        }

        [Test]
        public void Run_IteratedGridResult_IterationResultPassedToPrint()
        {
            var grid = GameOfLife.GetGrid(new bool[2, 2]);
            var iteratedGrid = GameOfLife.GetGrid(new bool[2, 2]);
            IEnumerable<Cell> actualGrid = null;

            Func<IEnumerable<Cell>, IEnumerable<Cell>> iterator = iterat => iteratedGrid;
            Action<IEnumerable<Cell>, int> print = (gridToPrint, iteration) => { actualGrid = gridToPrint; };

            GameOfLife.Run(grid, 1, iterator, print, () => { });

            Assert.That(actualGrid, Is.Not.SameAs(grid));
            Assert.That(actualGrid, Is.SameAs(iteratedGrid));
        }

        [Test]
        public void Neighbours_GridCellWithNoNeighbours_EmptyCollectionReturned()
        {
            var grid = GameOfLife.GetGrid(new bool[4, 4]);
            IEnumerable<Cell> neighbours = grid.Neighbours(new Cell(2, 2, false));

            Assert.That(neighbours.Count, Is.EqualTo(0));
        }

        [Test]
        public void Neighbours_GridCellWith3Neighbours_CollectionWith3CellsReturned()
        {
            var array = new bool[4, 4];
            array[1, 1] = true;
            array[1, 2] = true;
            array[2, 1] = true;
            var grid = GameOfLife.GetGrid(array);
            List<Cell> neighbours = grid.Neighbours(new Cell(2, 2, false));

            Assert.That(neighbours.Count, Is.EqualTo(3));
        }

        [Test, TestCaseSource(nameof(CheckProcreationTests))]
        public void CheckProcreationTest(List<Cell> neighbours, bool expected)
        {
            bool result = GameOfLife.CheckProcreation(neighbours);

            Assert.That(result, Is.EqualTo(expected));
        }

        private static IEnumerable<TestCaseData> CheckProcreationTests()
        {
            yield return new TestCaseData(GetRandomNeighbours(3), true).SetName("CheckProcreation_With3Neighbours_ReturnsTrue");
            yield return new TestCaseData(GetRandomNeighbours(4), false).SetName("CheckProcreation_With4Neighbours_ReturnsFalse");
            yield return new TestCaseData(GetRandomNeighbours(2), false).SetName("CheckProcreation_With2Neighbours_ReturnsFalse");
        }

        [Test, TestCaseSource(nameof(CheckOvercrowdingTests))]
        public void CheckOvercrowdingTest(List<Cell> neighbours, bool expected)
        {
            bool result = GameOfLife.CheckOvercrowding(neighbours);

            Assert.That(result, Is.EqualTo(expected));
        }

        private static IEnumerable<TestCaseData> CheckOvercrowdingTests()
        {
            yield return new TestCaseData(GetRandomNeighbours(3), false).SetName("CheckOvercrowding_With3Neighbours_ReturnsFalse");
            yield return new TestCaseData(GetRandomNeighbours(2), false).SetName("CheckOvercrowding_With2Neighbours_ReturnsFalse");
            yield return new TestCaseData(GetRandomNeighbours(4), true).SetName("CheckOvercrowding_With4Neighbours_ReturnsTrue");
        }

        [Test, TestCaseSource(nameof(CheckUnderpopulationTests))]
        public void CheckUnderpopulationTest(List<Cell> neighbours, bool expected)
        {
            bool result = GameOfLife.CheckUnderpopulation(neighbours);

            Assert.That(result, Is.EqualTo(expected));
        }

        private static IEnumerable<TestCaseData> CheckUnderpopulationTests()
        {
            yield return new TestCaseData(GetRandomNeighbours(3), false).SetName("CheckUnderpopulation_With3Neighbours_ReturnsFalse");
            yield return new TestCaseData(GetRandomNeighbours(2), false).SetName("CheckUnderpopulation_With2Neighbours_ReturnsFalse");
            yield return new TestCaseData(GetRandomNeighbours(1), true).SetName("CheckUnderpopulation_With1Neighbour_ReturnsTrue");
        }

        private static IEnumerable<Cell> GetRandomNeighbours(int numberOfNeighbours)
        {
            var neighbours = new List<Cell>();

            for (int i = 0; i < numberOfNeighbours; i++)
            {
                neighbours.Add(new Cell(0, 0, true));
            }

            return neighbours;
        }

        [Test]
        public void Iterate_WithTwoSquareGrid_ApplyConditionsCalledForEachCell()
        {
            IEnumerable<Cell> grid = GameOfLife.GetGrid(new bool[2, 2]);
            int applyConditionsCalledTimes = 0;


            Func<bool, IEnumerable<Cell>, bool> applyConditions = (cellState, neighbours) =>
            {
                applyConditionsCalledTimes++;
                return true;
            };

            IEnumerable<Cell> result = GameOfLife.Iterate(grid, applyConditions);

            Assert.That(result, Is.Not.SameAs(grid));
            Assert.That(applyConditionsCalledTimes, Is.EqualTo(4));
        }

        [Test]
        public void Print_WithPopulatedGrid_OutputIsFormatted()
        {
            bool[,] array = new bool[2, 2];
            array[0, 0] = true;
            array[1, 1] = true;
            IEnumerable<Cell> grid = GameOfLife.GetGrid(array);
            List<string> outputLines = new List<string>(3);
            Action<string> writeLine = line => outputLines.Add(line);

            GameOfLife.Print(writeLine, grid, 1);

            Assert.That(outputLines[0], Is.EqualTo("Iteration 1"));
            Assert.That(outputLines[1], Is.EqualTo("x."));
            Assert.That(outputLines[2], Is.EqualTo(".x"));

        }
    }
}
