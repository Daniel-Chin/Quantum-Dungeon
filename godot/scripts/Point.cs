using System;
public abstract class Point {}
public class Point<T> : Point {
    public T X;
    public T Y;
    public Point(T x, T y) {
        X = x;
        Y = y;
    }
    public override bool Equals(object obj) {
        if (obj is Point<T> other) {
            if (
                other.X is int oiX &&
                other.Y is int oiY &&
                      X is int tiX &&
                      Y is int tiY 
            ) {
                return (
                    tiX == oiX &&
                    tiY == oiY
                );
            } else if (
                other.X is double odX &&
                other.Y is double odY &&
                      X is double tdX &&
                      Y is double tdY 
            ) {
                return (
                    tdX == odX &&
                    tdY == odY
                );
            } else throw new Exception("gr35nu");
        } else {
            return false;
        }
    }
    public override int GetHashCode() {
        if (X is int ix && Y is int iy) {
            return ix * 128 + iy;
        } else if (X is double dx && Y is double dy) {
            return dx.GetHashCode() ^ dy.GetHashCode();
        } else throw new Exception("eri358");
    }
}
