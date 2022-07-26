using Godot;
using System;
using System.Collections.Generic;
class Raster {
    protected static double EpsToZero(double x) {
        // GD.PrintS(x);
        if (Math.Abs(x) < Double.Epsilon) {
            return 0;
        }
        return x;
    }
    public static PosNegMatrix BoundAndMatrix(List<Point> vertices) {
        int xStart = int.MaxValue; 
        int yStart = int.MaxValue; 
        int xEnd   = int.MinValue; 
        int yEnd   = int.MinValue;    
        foreach (Point p in vertices) {
            xStart = (int) Math.Min(xStart, Math.Floor  (p.X));
            yStart = (int) Math.Min(yStart, Math.Floor  (p.Y));
            xEnd   = (int) Math.Max(xEnd  , Math.Ceiling(p.X));
            yEnd   = (int) Math.Max(yEnd  , Math.Ceiling(p.Y));
        }
        // GD.PrintS(
        //     xStart, 
        //     yStart, 
        //     xEnd, 
        //     yEnd 
        // );
        return new PosNegMatrix(
            xStart - 1,
            yStart - 1,
            xEnd   + 1,
            yEnd   + 1
        );
    }
    public static void LineSeg(
        LineSegment lSeg, 
        PosNegMatrix output
    ) {
        Point Start = lSeg.Start;
        Point End   = lSeg.End;
        int xSign = Math.Sign(lSeg.Vector.X);
        double Slope = lSeg.Slope;

        // GD.PrintS(lSeg, Slope);
        if (Math.Abs(Slope) < 1) {
            double YIntercept = Start.Y - Start.X * Slope;
            int xMin = (int) Math.Ceiling(Math.Min(Start.X, End.X));
            int xMax = (int) Math.Floor  (Math.Max(Start.X, End.X));
            for (int x = xMin; x <= xMax; x ++) {
                double y = x * Slope + YIntercept;
                int intY = (int) Math.Floor(y);
                output[x - 1, intY] = true;
                output[x    , intY] = true;
                if (Math.Abs(y - intY) < double.Epsilon) {
                    output[x - 1, intY - 1] = true;
                    output[x    , intY - 1] = true;
                }
            }
        } else {
            double inverseSlope = lSeg.Vector.X / lSeg.Vector.Y;
            double XIntercept = Start.X - Start.Y * inverseSlope;
            // GD.PrintS(inverseSlope, XIntercept);
            int yMin = (int) Math.Ceiling(Math.Min(Start.Y, End.Y));
            int yMax = (int) Math.Floor  (Math.Max(Start.Y, End.Y));
            for (int y = yMin; y <= yMax; y ++) {
                double x = y * inverseSlope + XIntercept;
                int intX = (int) Math.Floor(x);
                output[intX, y - 1] = true;
                output[intX, y    ] = true;
                if (Math.Abs(x - intX) < double.Epsilon) {
                    output[intX - 1, y - 1] = true;
                    output[intX - 1, y    ] = true;
                }
            }
        }
    }
    public static PosNegMatrix RasterOnlyEdges(
        List<Point> vertices
    ) {
        // Debug only. Does not even close the loop.
        PosNegMatrix output = BoundAndMatrix(vertices);
        Point last = null;
        foreach (Point p in vertices) {
            if (last != null) {
                LineSegment lSeg = new LineSegment(last, p);
                LineSeg(lSeg, output);
            }
            last = p;
        }
        return output;
    }
    public static PosNegMatrix Main(
        List<Point> vertices, Point eye
    ) {
        // foreach (Point p in vertices) GD.PrintS(" ", p);
        PosNegMatrix output = BoundAndMatrix(vertices);
        PosNegMatrix[] radiations = new PosNegMatrix[vertices.Count];
        {
            int i = 0;
            foreach (Point p in vertices) {
                PosNegMatrix pnM = output.Birth();
                LineSeg(new LineSegment(
                    eye, p
                ), pnM);
                radiations[i ++] = pnM;
            }
        }
        ;
        foreach (
            Tuple<Tuple<Point, PosNegMatrix>, Tuple<Point, PosNegMatrix>> pair in 
            SelfPairUp<Tuple<Point, PosNegMatrix>>.Main(
                Zip<Point, PosNegMatrix>.Main(vertices, radiations)
            )
        ) {
            Tuple<Point, PosNegMatrix> prev = pair.Item1;
            Tuple<Point, PosNegMatrix> next = pair.Item2;
            PosNegMatrix triangle = output.Birth();
            triangle.InplaceOr(prev.Item2);
            triangle.InplaceOr(next.Item2);
            LineSeg(new LineSegment(
                prev.Item1, next.Item1
            ), triangle);
            FillTriangle(triangle);
            output.InplaceOr(triangle);
        }
        return output;
    }
    protected static void FillTriangle(PosNegMatrix triangle) {
        for (int x = triangle.XStart; x < triangle.XEnd; x ++) {
            int yStart;
            for (
                yStart = triangle.YStart; 
                yStart < triangle.YEnd; yStart ++
            ) {
                if (triangle[x, yStart]) break;
            }
            int yEnd;
            for (
                yEnd = triangle.YEnd - 1; 
                yEnd >= yStart; yEnd --
            ) {
                if (triangle[x, yEnd]) break;
            }
            for (int y = yStart + 1; y < yEnd; y ++) {
                triangle[x, y] = true;
            }
        }
    }
}
