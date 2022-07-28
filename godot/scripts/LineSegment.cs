using Godot;
using System;
using System.Collections.Generic;

public class LineSegment : IComparable<LineSegment> {
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
    public double Length {
        get; protected set;
    }
    public double Reduce {
        get; protected set;
    }

    public LineSegment(Point one, Point another) {
        if (one.LessThan(another)) {
            Start = one;
            End   = another;
        } else {
            Start = another;
            End   = one;
        }
        double dx = End.X - Start.X;
        double dy = End.Y - Start.Y;
        Vector = new Point(dx, dy);
        Slope = dy / dx;
        Length = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        Reduce = (
            + Start.X 
            + Start.Y * IRRATIONAL_0
            + End  .X * IRRATIONAL_1
            + End  .Y * IRRATIONAL_2
        );     
    }
    public override string ToString() {
        return $"LineSeg[{Start} - {End} L1={ManhattanMag}]";
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
    protected static readonly double IRRATIONAL_0 = Math.Sqrt(2);
    protected static readonly double IRRATIONAL_1 = Math.Sqrt(3);
    protected static readonly double IRRATIONAL_2 = Math.Sqrt(5);
    int IComparable<LineSegment>.CompareTo(LineSegment that) {
        // GD.PrintS(" ", this, "vs", that);
        if (Equals(that)) {
            // GD.PrintS("  Equal");
            // GD.PrintS(" ", this, "==", that);
            return 0;
        }
        int sign = ManhattanMag.CompareTo(that.ManhattanMag);
        // GD.PrintS(" ", sign);
        if (sign == 0) {
            // GD.PrintS(" L1 is both", ManhattanMag);
            return Reduce.CompareTo(that.Reduce);
        } else {
            return sign;
        }
    }
    public bool Equals(LineSegment that) {
        return (
            Start.Equals(that.Start) &&
            End  .Equals(that.End  ) ||
            Start.Equals(that.End  ) &&
            End  .Equals(that.Start)
        );
    }
    public override bool Equals(object obj) {
        return false;
    }
    public override int GetHashCode() {
        return Start.GetHashCode() ^ End.GetHashCode();
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
        double B = Start.X - End.X;
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
