using System;
using System.Collections.Generic;
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
    }
}
