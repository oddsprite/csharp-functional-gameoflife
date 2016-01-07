using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public static class GameOfLife
    {
        public const bool Alive = true;
        public const bool Dead = false;

        public static void Run(bool[,] grid, int iterations)
        {
            Print(Console.WriteLine, grid, 0);
            Console.WriteLine("Press Enter To Begin");
            Console.ReadLine();

            for (int iteration = 1; iteration < iterations; iteration++)
            {
                grid = Iterate(grid);
                Print(Console.WriteLine, grid, iteration);
                Task.Delay(250).Wait();
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
                    bool cellAlive = grid[i, j];
                    bool[] neighbours = grid.Neighbours(i, j);

                    if (cellAlive && CheckUnderpopulation(neighbours))
                    {
                        resultGrid[i, j] = Dead;
                    }
                    else if (cellAlive && CheckOvercrowding(neighbours))
                    {
                        resultGrid[i, j] = Dead;
                    }
                    else if (cellAlive && CheckNextGeneration(neighbours))
                    {
                        resultGrid[i, j] = Alive;
                    }
                    else if (!cellAlive && CheckProcreation(neighbours))
                    {
                        resultGrid[i, j] = Alive;
                    }
                }
            }

            return resultGrid;
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

        public static void Print(Action<string> writeLine, bool[,] grid, int iteration)
        {
            Console.Clear();
            writeLine($"Iteration {iteration}");
            PrintGrid(writeLine, grid);
        }
    }
}
