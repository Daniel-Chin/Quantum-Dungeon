using System;
public class Point : IComparable<Point> {
    public static readonly double EPS = 1e-7;
    public static readonly double THIRD_INVERSE_EPS;
    static Point() {
        THIRD_INVERSE_EPS = 1 / EPS / 3;
    }
    public double X {
        get; private set; 
    }
    public double Y {
        get; private set; 
    }
    public Point(double x, double y) {
        X = x;
        Y = y;
    }
    public override string ToString() {
        return $"({X}, {Y})";
    }
    int IComparable<Point>.CompareTo(Point that) {
        if (this.X == that.X) {
            if (this.Y == that.Y) {
                return 0;
            } else if (this.Y < that.Y) {
                return -1;
            } else {
                return +1;
            }
        } else if (this.X < that.X) {
            return -1;
        } else {
            return +1;
        }
    }
    public bool LessThan(Point that) {
        return ((IComparable<Point>) this).CompareTo(that) < 0;
    }
    public override bool Equals(object obj) {
        if (obj is Point other) {
            return X == other.X && Y == other.Y;
        } else {
            throw new Exception("ge8h43q0");
        }
    }
    public override int GetHashCode() {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
    public Point Offset05() {
        return new Point(X + .5, Y + .5);
    }
    public double ManhattanMag() {
        return Math.Abs(X) + Math.Abs(Y);
    }
}

public class PointInt : Point {
    public int IntX {
        get; private set; 
    }
    public int IntY {
        get; private set; 
    }
    public PointInt(int x, int y) : base(x, y) {
        IntX = x;
        IntY = y;
    }
    public override string ToString() {
        return $"({IntX}, {IntY})";
    }
    public void Deconstruct(out int x, out int y) {
        x = IntX;
        y = IntY;
    }
}
