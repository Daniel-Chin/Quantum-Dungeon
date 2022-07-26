using System;
using System.Collections.Generic;

public class LineSegment<T> : IComparable {
    public Point<T> Start {
        get; protected set;
    }
    public Point<T> End {
        get; protected set;
    }
    public int FourManhattanMag {
        get; protected set;
    }
    public Point<double> Vector {
        get; protected set;
    }
    public double Slope {
        get; protected set;
    }
    public double Intercept {
        get; protected set;
    }
    public double Length {
        get; protected set;
    }

    public LineSegment(
        Point<T> start,  
        Point<T> end 
    ) {
        Type t = typeof(T);
        if (t != typeof(int) && t != typeof(double))
            throw new Exception("wrhg53");
        Start = start;
        End   = end;
        if (
            start.X is double sX && 
            start.Y is double sY && 
            end  .X is double eX && 
            end  .Y is double eY
        ) {
            double dx = eX - sX;
            double dy = eY - sY;
            Vector = new Point<Double>(dx, dy);
            Slope = dy / dx;
            Length = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            Intercept = sY - sX * Slope;
        }
    }
    public void UpdateManhattanMag(Point<int> playerPos) {
        if (
            Start.X is int sX &&
            Start.Y is int sY &&
            End  .X is int eX &&
            End  .Y is int eY
        ) {
            FourManhattanMag = (
                + Math.Abs(2 * (sX - playerPos.X) - 1)
                + Math.Abs(2 * (eX - playerPos.X) - 1)
                + Math.Abs(2 * (sY - playerPos.Y) - 1)
                + Math.Abs(2 * (eY - playerPos.Y) - 1)
            );
        }
    }
    public int CompareTo(object obj) {
        if (obj is LineSegment<int> other) {
            return FourManhattanMag.CompareTo(other.FourManhattanMag);
        } else {
            throw new ArgumentException();
        }
    }
    public override bool Equals(object obj) {
        if (obj is LineSegment<T> other) {
            return (
                Start.Equals(other.Start) &&
                End  .Equals(other.End  )
            );
        } else {
            return false;
        }
    }
    public override int GetHashCode() {
        return Start.GetHashCode() << 14 ^ End.GetHashCode();
    }

    public void Rasterize(
        Dictionary<Tuple<int, int>, bool> output
    ) {
        if (
            Start.X is double sX &&
            Start.Y is double sY &&
            End  .X is double eX &&
            End  .Y is double eY
        ) {
            double inverseSlope = 1 / Slope;
            int xSign = Math.Sign(Vector.X);
            int ySign = Math.Sign(Vector.Y);
            if (xSign == 0 && ySign == 0)
                return;
            Point<Double> UnitVector = new Point<Double>(
                Vector.X / Length, 
                Vector.Y / Length
            );
            int cellX = (int) Math.Floor(sX);
            int cellY = (int) Math.Floor(sY);
            double residualX = sX - cellX;
            double residualY = sY - cellY;
            while (true) {
                output[new Tuple<int, int>(cellX, cellY)] = true;
                {
                    double x = cellX + residualX;
                    double y = cellY + residualY;
                    x -= eX;
                    y -= eY;
                    if (x * Vector.X + y * Vector.Y >= 0)
                        break;
                }
                double armX = UnitVector.X * 2;
                double armY = UnitVector.Y * 2;
                double handX = residualX + armX;
                double handY = residualY + armY;
                int sign_00 = Math.Sign(
                    armY * 0 - handX - armX * 0 - handY
                );
                int sign_01 = Math.Sign(
                    armY * 0 - handX - armX * 1 - handY
                );
                int sign_10 = Math.Sign(
                    armY * 1 - handX - armX * 0 - handY
                );
                int sign_11 = Math.Sign(
                    armY * 1 - handX - armX * 1 - handY
                );
                int moveX = 0;
                int moveY = 0;
                if      (sign_00 < sign_01) 
                    moveX = -1;
                else if (sign_01 < sign_11) 
                    moveY = +1;
                else if (sign_11 < sign_10) 
                    moveX = +1;
                else if (sign_10 < sign_00) 
                    moveY = -1;
                else 
                    throw new Exception("ghe9p8hret534");
                cellX += moveX;
                cellY += moveY;
                if (cellX != 0) {
                    residualX = 0;
                    residualY = (
                        cellX * Slope + Intercept - cellY
                    );
                } else {
                    residualY = 0;
                    residualX = (
                        cellY - Intercept
                    ) * inverseSlope - cellX;
                }
            }
        }
    }
}
