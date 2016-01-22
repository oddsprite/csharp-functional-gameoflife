namespace GameOfLife
{
    public struct Cell
    {
        public readonly int X;
        public readonly int Y;
        public bool SwitchedOn;

        public Cell(int x, int y, bool switchedOn)
        {
            this.X = x;
            this.Y = y;
            this.SwitchedOn = switchedOn;
        }

        public override string ToString()
        {
            return $"({X},{Y}) = {SwitchedOn}";
        }
    }
}
