using System;

namespace GameOfLife
{
    public static class Program
    {
        static void Main(string[] args)
        {
            bool[,] grid = new bool[6, 12];

            grid[1, 4] = true;
            grid[2, 3] = true;
            grid[2, 4] = true;
            grid[2, 5] = true;
            grid[3, 4] = true;
            
            GameOfLife.Run(grid, 1000);

            Console.ReadLine();
        }
    }
}
