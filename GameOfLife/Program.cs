using System;
using System.Threading.Tasks;

namespace GameOfLife
{
    public static class Program
    {
        static void Main(string[] args)
        {
            bool[,] array = new bool[6, 12];

            array[1, 4] = true;
            array[2, 3] = true;
            array[2, 4] = true;
            array[2, 5] = true;
            array[3, 4] = true;

            var grid = GameOfLife.GetGrid(array);

            GameOfLife.Print(Console.WriteLine, grid, 0);
            Console.WriteLine("Press Enter To Begin");
            Console.ReadLine();

            GameOfLife.Run(
                grid: grid,
                iterations: 1000,
                iterator: (g) => GameOfLife.Iterate(g, GameOfLife.ApplyConditions),
                print: (gridToPrint, iteration) => GameOfLife.Print(Console.WriteLine, gridToPrint, iteration, clear: Console.Clear),
                postIteration: () => Task.Delay(250).Wait());

            Console.ReadLine();
        }
    }
}
