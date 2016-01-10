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
        [Test, TestCaseSource(nameof(OnGridTests))]
        public void OnGridTest(int x, int y, IResolveConstraint expectation)
        {
            bool[,] grid = new bool[3, 3];
            Assert.That(grid.OnGrid(y, x), expectation);
        }

        private static IEnumerable<TestCaseData> OnGridTests()
        {
            yield return new TestCaseData(1, 1, Is.True).SetName("OnGrid_CoordinatesExistOnGrid_True");
            yield return new TestCaseData(1, -1, Is.False).SetName("OnGrid_CoordinatesOffGridNegativeX_False");
            yield return new TestCaseData(1, 3, Is.False).SetName("OnGrid_CoordinatesOffGridPositiveX_False");
            yield return new TestCaseData(-1, 1, Is.False).SetName("OnGrid_CoordinatesOffGridNegativeY_False");
            yield return new TestCaseData(3, 1, Is.False).SetName("OnGrid_CoordinatesOffGridPositiveY_False");
        }

        [Test]
        public void Run_With3Iterations_AllIterationsRun()
        {
            var grid = new bool[1, 1];
            int iterations = 3;
            int actualIterations = 0;

            Func<bool[,], bool[,]> iterator = iterat =>
            {
                actualIterations++;
                return new bool[1, 1];
            };

            GameOfLife.Run(grid, iterations, iterator, (bools, i) => { }, () => { });

            Assert.That(actualIterations, Is.EqualTo(iterations));
        }

        [Test]
        public void Run_IteratedGridResult_IterationResultPassedToPrint()
        {
            var grid = new bool[2, 2];
            var iteratedGrid = new bool[2, 2];
            bool[,] actualGrid = null;

            Func<bool[,], bool[,]> iterator = iterat => iteratedGrid;
            Action<bool[,], int> print = (gridToPrint, iteration) => { actualGrid = gridToPrint; };

            GameOfLife.Run(grid, 1, iterator, print, () => { });

            Assert.That(actualGrid, Is.Not.SameAs(grid));
            Assert.That(actualGrid, Is.SameAs(iteratedGrid));
        }

        [Test]
        public void Neighbours_GridCellWithNoNeighbours_EmptyArrayReturned()
        {
            var grid = new bool[4, 4];
            bool[] neighbours = grid.Neighbours(2, 2);

            Assert.That(neighbours.Length, Is.EqualTo(8));
            Assert.That(neighbours.Count(n => n), Is.EqualTo(0));
        }

        [Test]
        public void Neighbours_GridCellWith3Neighbours_ArrayLength8With3TrueReturned()
        {
            var grid = new bool[4, 4];
            grid[1, 1] = true;
            grid[1, 2] = true;
            grid[2, 1] = true;
            bool[] neighbours = grid.Neighbours(2, 2);

            Assert.That(neighbours.Length, Is.EqualTo(8));
            Assert.That(neighbours.Count(n => n), Is.EqualTo(3));
        }

        [Test, TestCaseSource(nameof(CheckProcreationTests))]
        public void CheckProcreationTest(bool[] neighbours, bool expected)
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

        [Test, TestCaseSource(nameof(CheckNextGenerationTests))]
        public void CheckNextGenerationTest(bool[] neighbours, bool expected)
        {
            bool result = GameOfLife.CheckNextGeneration(neighbours);

            Assert.That(result, Is.EqualTo(expected));
        }

        private static IEnumerable<TestCaseData> CheckNextGenerationTests()
        {
            yield return new TestCaseData(GetRandomNeighbours(3), true).SetName("CheckNextGeneration_With3Neighbours_ReturnsTrue");
            yield return new TestCaseData(GetRandomNeighbours(2), true).SetName("CheckNextGeneration_With2Neighbours_ReturnsTrue");
            yield return new TestCaseData(GetRandomNeighbours(1), false).SetName("CheckNextGeneration_With1Neighbour_ReturnsFalse");
            yield return new TestCaseData(GetRandomNeighbours(4), false).SetName("CheckNextGeneration_With4Neighbours_ReturnsFalse");
        }

        [Test, TestCaseSource(nameof(CheckOvercrowdingTests))]
        public void CheckOvercrowdingTest(bool[] neighbours, bool expected)
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
        public void CheckUnderpopulationTest(bool[] neighbours, bool expected)
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

        private static bool[] GetRandomNeighbours(int numberOfNeighbours)
        {
            var neighbours = new bool[8];

            for (int i = 0; i < numberOfNeighbours; i++)
            {
                neighbours[i] = true;
            }
            
            return neighbours;
        }
    }
}
