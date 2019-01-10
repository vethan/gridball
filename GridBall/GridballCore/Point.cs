using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballCore
{
    public struct Point 
    {
        public int x;
        public int y;

        public enum Direction { Up, Down, Left, Right }


        public static Point operator *(Point one, int other)
        {
            return new Point(one.x * other, one.y * other);
        }


        public static Point operator +(Point one,Point other)
        {
            return new Point(one.x + other.x, one.y + other.y);
        }


        public static Point operator -(Point one, Point other)
        {
            return new Point(one.x - other.x, one.y - other.y);
        }

        public static Point FromDirection(Direction direction)
        {
            switch(direction)
            {
                case Direction.Up:
                    return new Point(0, 1);
                case Direction.Down:
                    return new Point(0, -1);
                case Direction.Left:
                    return new Point(-1, 0);
                case Direction.Right:
                    return new Point(1, 0);
            }
            throw new NotImplementedException();
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point ClampTo(Point min, Point max)
        {
            this.x = Math.Max(this.x, min.x);
            this.x = Math.Min(this.x, max.x);

            this.y = Math.Max(this.y, min.y);
            this.y = Math.Min(this.y, max.y);
            return this;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(x).And(y);
        }
    }
}
