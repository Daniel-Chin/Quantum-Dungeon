using System;
using System.Collections.Generic;

public class LineSegment : IComparable {
    public Point Start {
        get; protected set;
    }
    public Point End {
        get; protected set;
    }
    public double ManhattanMag {
        get; protected set;
    }
    public Point Vector {
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

    public LineSegment(Point start, Point end) {
        Start = start;
        End   = end;
        double dx = end.X - start.X;
        double dy = end.Y - start.Y;
        Vector = new Point(dx, dy);
        Slope = dy / dx;
        Length = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        Intercept = start.Y - start.X * Slope;
    }
    public void UpdateManhattanMag(Point EyePos) {
        ManhattanMag = (
            Math.Abs((
                Start.X + End.X - 2 * EyePos.X
            ) * .5) + 
            Math.Abs((
                Start.Y + End.Y - 2 * EyePos.Y
            ) * .5)
        );
    }
    int IComparable.CompareTo(object obj) {
        if (obj is LineSegment other) {
            return ManhattanMag.CompareTo(other.ManhattanMag);
        } else {
            throw new ArgumentException();
        }
    }
    public override bool Equals(object obj) {
        if (obj is LineSegment other) {
            return (
                Start.Equals(other.Start) &&
                End  .Equals(other.End  ) ||
                Start.Equals(other.End  ) &&
                End  .Equals(other.Start)
            );
        } else {
            return false;
        }
    }
    public override int GetHashCode() {
        return Start.GetHashCode() ^ End.GetHashCode();
    }

    public void Rasterize(
        Dictionary<PointInt, bool> output
    ) {
        double inverseSlope = 1 / Slope;
        int xSign = Math.Sign(Vector.X);
        int ySign = Math.Sign(Vector.Y);
        if (xSign == 0 && ySign == 0)
            return;
        Point UnitVector = new Point(
            Vector.X / Length, 
            Vector.Y / Length
        );
        int cellX = (int) Math.Floor(Start.X);
        int cellY = (int) Math.Floor(Start.Y);
        double residualX = Start.X - cellX;
        double residualY = Start.Y - cellY;
        while (true) {
            output[new PointInt(cellX, cellY)] = true;
            {
                double x = cellX + residualX;
                double y = cellY + residualY;
                x -= End.X;
                y -= End.Y;
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
    public Point IntersectWith(LineSegment that) {
        // Extends the segments into lines. 
        var (A0, B0, C0) = ABC();
        var (A1, B1, C1) = that.ABC();
        double delta = A0 * B1 - A1 * B0;
        if (delta == 0) 
            throw new Exception("Parallel lines");
        return new Point(
            (B1 * C0 - B0 * C1) / delta, 
            (A0 * C1 - A1 * C0) / delta
        );
    }
    protected Tuple<double, double, double> ABC() {
        double A = End.Y - Start.Y;
        double B = End.X - Start.X;
        double C = A * Start.X + B * Start.Y;
        return new Tuple<double, double, double>(A, B, C);
    }
}

public class LineSegmentInt : LineSegment{
    public LineSegmentInt(
        PointInt start, PointInt end
    ) : base(start, end) {
    }
}
