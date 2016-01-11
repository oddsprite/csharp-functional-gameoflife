using System;
using System.Threading.Tasks;

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

            GameOfLife.Print(Console.WriteLine, grid, 0);
            Console.WriteLine("Press Enter To Begin");
            Console.ReadLine();

            GameOfLife.Run(
                grid,
                1000,
                (g) => GameOfLife.Iterate(g, GameOfLife.ApplyConditions), 
                (gridToPrint, iteration) => GameOfLife.Print(Console.WriteLine, gridToPrint, iteration, clear: Console.Clear),
                () => Task.Delay(250).Wait());

            Console.ReadLine();
        }
    }
}
