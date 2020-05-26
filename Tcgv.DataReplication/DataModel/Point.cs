namespace Tcgv.DataReplication.DataModel
{
    public class Point
    {
        public Point(long x, long y)
        {
            X = (int)x;
            Y = (int)y;
        }

        public int X { get; }
        public int Y { get; }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + X.GetHashCode();
            hash = (hash * 7) + Y.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            var p = obj as Point;
            return p is Point &&
                p.X == X && p.Y == Y;
        }
    }
}
