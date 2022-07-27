using System;
public class Point {
    public double X {
        get; private set; 
    }
    public double Y {
        get; private set; 
    }
    public bool IsInt {
        get; private set; 
    }
    private int intX;
    public int IntX {
        get {
            new Assert(IsInt);
            return intX;
        }
    }
    private int intY;
    public int IntY {
        get {
            new Assert(IsInt);
            return intY;
        }
    }
    public Point(double x, double y) {
        X = x;
        Y = y;
        IsInt = false;
    }
    public Point(int x, int y) {
        X = (double) x;
        Y = (double) y;
        IsInt = true;
        intX = x;
        intY = y;
    }
    public override bool Equals(object obj) {
        if (obj is Point other) {
            return (
                Math.Abs(X - other.X) < double.Epsilon &&
                Math.Abs(Y - other.Y) < double.Epsilon
            );
        } else {
            return false;
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
