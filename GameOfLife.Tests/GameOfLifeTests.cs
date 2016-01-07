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
    }
}
