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

        public static bool[,] Iterate(bool[,] grid, Func<GameState, bool[,], bool> applyConditions)
        {
            bool[,] resultGrid = new bool[grid.GetLength(0), grid.GetLength(1)];
            Array.Copy(grid, resultGrid, grid.Length);

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var gameState = new GameState(grid[i, j], grid.Neighbours(i, j), i, j);
                    applyConditions(gameState, resultGrid);
                }
            }

            return resultGrid;
        }

        public static bool ApplyConditions(GameState inputState, bool[,] resultGrid)
        {
            return inputState.CellState && CheckUnderpopulation(inputState.Neighbours)
                ? resultGrid[inputState.Y, inputState.X] = Dead
                : inputState.CellState && CheckOvercrowding(inputState.Neighbours)
                    ? resultGrid[inputState.Y, inputState.X] = Dead
                    : inputState.CellState && CheckNextGeneration(inputState.Neighbours)
                        ? resultGrid[inputState.Y, inputState.X] = Alive
                        : !inputState.CellState && CheckProcreation(inputState.Neighbours) && (resultGrid[inputState.Y, inputState.X] = Alive);
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

        public class GameState
        {
            public bool CellState { get; }
            public bool[] Neighbours { get; }
            public int Y { get; }
            public int X { get; }

            public GameState(bool cellState, bool[] neighbours, int y, int x)
            {
                this.CellState = cellState;
                this.Neighbours = neighbours;
                this.Y = y;
                this.X = x;
            }
        }
    }
}
