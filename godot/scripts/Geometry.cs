using System;
using System.Collections.Generic;

public class Geometry {
    public class LineSegment {
        public Tuple<double, double> Start {
            get; protected set;
        }
        public Tuple<double, double> End {
            get; protected set;
        }
        public Tuple<double, double> Vector {
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
            Tuple<double, double> start, 
            Tuple<double, double> end
        ) {
            Start = start;
            End   = end;
            double dx = end.Item1 - start.Item1;
            double dy = end.Item2 - start.Item2;
            Vector = new Tuple<double, double>(dx, dy);
            Slope = dy / dx;
            Length = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            Intercept = start.Item2 - start.Item1 * Slope;
        }

        public void Rasterize(
            Dictionary<Tuple<int, int>, bool> output
        ) {
            double inverseSlope = 1 / Slope;
            int xSign = Math.Sign(Vector.Item1);
            int ySign = Math.Sign(Vector.Item2);
            if (xSign == 0 && ySign == 0)
                return;
            Tuple<double, double> UnitVector = new Tuple<double, double>(
                Vector.Item1 / Length, 
                Vector.Item2 / Length
            );
            int cellX = (int) Math.Floor(Start.Item1);
            int cellY = (int) Math.Floor(Start.Item2);
            double residualX = Start.Item1 - cellX;
            double residualY = Start.Item2 - cellY;
            while (true) {
                output[new Tuple<int, int>(cellX, cellY)] = true;
                {
                    double x = cellX + residualX;
                    double y = cellY + residualY;
                    x -= End.Item1;
                    y -= End.Item2;
                    if (x * Vector.Item1 + y * Vector.Item2 >= 0)
                        break;
                }
                double armX = UnitVector.Item1 * 2;
                double armY = UnitVector.Item2 * 2;
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
