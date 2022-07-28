using Godot;
using System;
using System.Collections.Generic;
class Raster {
    protected static double EpsToZero(double x) {
        GD.PrintS(x);
        if (Math.Abs(x) < Double.Epsilon) {
            return 0;
        }
        return x;
    }
    public static void LineSeg(
        LineSegment lSeg, 
        Dictionary<PointInt, bool> output
    ) {
        Point Start = lSeg.Start;
        Point End   = lSeg.End;
        double Slope = lSeg.Slope;
        double inverseSlope = 1 / Slope;
        Point Vector = lSeg.Vector;
        double Length = lSeg.Length;
        double Intercept = lSeg.Intercept;
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
            int sign_00 = Math.Sign(EpsToZero(
                armY * 0 - handX - armX * 0 - handY
            ));
            int sign_01 = Math.Sign(EpsToZero(
                armY * 0 - handX - armX * 1 - handY
            ));
            int sign_10 = Math.Sign(EpsToZero(
                armY * 1 - handX - armX * 0 - handY
            ));
            int sign_11 = Math.Sign(EpsToZero(
                armY * 1 - handX - armX * 1 - handY
            ));
            // GD.PrintS(sign_00, sign_01, sign_10, sign_11);
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
            if (moveX != 0) {
                residualX = 0;
                residualY = (
                    cellX * Slope + Intercept - cellY
                );
            } else {
                residualY = 0;
                if (xSign != 0) {
                    residualX = (
                        cellY - Intercept
                    ) * inverseSlope - cellX;
                }
            }
        }
    }
    public static void RasterOnlyEdges(
        List<Point> vertices, 
        Dictionary<PointInt, bool> output
    ) {
        // Debug only. Does not even close the loop.
        Point last = null;
        foreach (Point p in vertices) {
            if (last != null) {
                LineSegment lSeg = new LineSegment(last, p);
                LineSeg(lSeg, output);
            }
            last = p;
        }
    }
}
