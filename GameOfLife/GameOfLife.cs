using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLife
{
    public static class GameOfLife
    {
        public static void Run(
            IEnumerable<Cell> grid,
            int iterations,
            Func<IEnumerable<Cell>, IEnumerable<Cell>> iterator,
            Action<IEnumerable<Cell>, int> print,
            Action postIteration)
        {
            for (int iteration = 1; iteration <= iterations; iteration++)
            {
                grid = iterator(grid);
                print(grid, iteration);
                postIteration();
            }
        }

        public static IEnumerable<Cell> Iterate(IEnumerable<Cell> grid, Func<bool, List<Cell>, bool> applyConditions)
        {
            // NOTE - need to call ToList to ensure it is fully evaluated.
            return grid.Select(cell => new Cell(cell.X, cell.Y, applyConditions(cell.SwitchedOn, grid.Neighbours(cell)))).ToList();
        }

        public static bool ApplyConditions(bool cellState, List<Cell> neighbours)
        {
            return cellState ?
                    !CheckUnderpopulation(neighbours) && !CheckOvercrowding(neighbours) :
                    CheckProcreation(neighbours);
        }

        public static bool CheckProcreation(List<Cell> neighbours)
        {
            return neighbours.Count == 3;
        }

        public static bool CheckOvercrowding(List<Cell> neighbours)
        {
            return neighbours.Count > 3;
        }

        public static bool CheckUnderpopulation(List<Cell> neighbours)
        {
            return neighbours.Count < 2;
        }

        public static List<Cell> Neighbours(this IEnumerable<Cell> grid, Cell cell)
        {
            return grid.Where(otherCell => cell.IsNextTo(otherCell) && otherCell.SwitchedOn).ToList();
        }

        public static bool IsNextTo(this Cell cell, Cell other)
        {
            var dx = Math.Abs(cell.X - other.X);
            var dy = Math.Abs(cell.Y - other.Y);
            return (dx == 0 || dx == 1) && (dy == 0 || dy == 1) && (dx == 1 || dy == 1);
        }

        public static void PrintGrid(Action<string> writeLine, IEnumerable<Cell> grid)
        {
            grid.Select(cell => cell.X)
                .Distinct()
                .OrderBy(i => i)
                .ToList()
                .ForEach(i => writeLine(string.Concat(grid
                    .Where(item => item.X == i)
                    .OrderBy(item => item.Y)
                    .Select(item => item.SwitchedOn ? "x" : ".")))
                );
        }

        public static void Print(Action<string> writeLine, IEnumerable<Cell> grid, int iteration, Action clear = null)
        {
            clear?.Invoke();

            writeLine($"Iteration {iteration}");
            PrintGrid(writeLine, grid);
        }

        public static IEnumerable<Cell> GetGrid(bool[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    yield return new Cell(i, j, grid[i, j]);
                }
            }
        }
    }
}
