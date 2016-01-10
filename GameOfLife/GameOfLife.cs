using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOfLife
{
    public static class GameOfLife
    {
        private const bool Alive = true;
        private const bool Dead = false;

        public static void Run(
            bool[,] grid,
            int iterations,
            Func<bool[,], bool[,]> iterator,
            Action<bool[,], int> print,
            Action postIteration)
        {
            for (int iteration = 1; iteration <= iterations; iteration++)
            {
                grid = iterator(grid);
                print(grid, iteration);
                postIteration();
            }
        }

        public static bool[,] Iterate(bool[,] grid)
        {
            bool[,] resultGrid = new bool[grid.GetLength(0), grid.GetLength(1)];
            Array.Copy(grid, resultGrid, grid.Length);

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var cellState = Tuple.Create(grid[i, j], grid.Neighbours(i, j), i, j);

                    ApplyConditions(cellState, resultGrid);
                }
            }

            return resultGrid;
        }

        public static bool ApplyConditions(Tuple<bool, bool[], int, int> inputState, bool[,] resultGrid)
        {
            return A.Match(() => inputState.Item1 && CheckUnderpopulation(inputState.Item2), () => resultGrid[inputState.Item3, inputState.Item4] = Dead) ||
                A.Match(() => inputState.Item1 && CheckOvercrowding(inputState.Item2), () => resultGrid[inputState.Item3, inputState.Item4] = Dead) ||
                A.Match(() => inputState.Item1 && CheckNextGeneration(inputState.Item2), () => resultGrid[inputState.Item3, inputState.Item4] = Alive) ||
                A.Match(() => !inputState.Item1 && CheckProcreation(inputState.Item2), () => resultGrid[inputState.Item3, inputState.Item4] = Alive);
        }

        public static bool CheckProcreation(bool[] neighbours)
        {
            return neighbours.Count(n => n) == 3;
        }

        public static bool CheckNextGeneration(bool[] neighbours)
        {
            return new[] { 2, 3 }.Contains(neighbours.Count(n => n));
        }

        public static bool CheckOvercrowding(bool[] neighbours)
        {
            return neighbours.Count(n => n) > 3;
        }

        public static bool CheckUnderpopulation(bool[] neighbours)
        {
            return neighbours.Count(n => n) < 2;
        }

        public static bool[] Neighbours(this bool[,] grid, int y, int x)
        {
            List<bool> neighbours = new List<bool>();

            for (int i = y - 1; i <= y + 1; i++)
            {
                for (int j = x - 1; j <= x + 1; j++)
                {
                    if (grid.OnGrid(i, j) && !(i == y && j == x))
                    {
                        neighbours.Add(grid[i, j]);
                    }
                }
            }

            return neighbours.ToArray();
        }

        public static bool OnGrid(this bool[,] grid, int y, int x)
        {
            return y >= 0 && x >= 0 && y < grid.GetLength(0) && x < grid.GetLength(1);
        }

        public static void PrintGrid(Action<string> writeLine, bool[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                StringBuilder line = new StringBuilder(5);

                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    line.Append(grid[i, j] ? "x" : ".");
                }

                writeLine(line.ToString());
            }
        }

        public static void Print(Action<string> writeLine, bool[,] grid, int iteration, Action clear = null)
        {
            clear?.Invoke();

            writeLine($"Iteration {iteration}");
            PrintGrid(writeLine, grid);
        }
    }
}
