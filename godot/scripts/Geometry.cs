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
        }

        public void Rasterize(
            Dictionary<Tuple<int, int>, bool> output
        ) {
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
                double restX;
                double lenX;
                if (xSign == 0) {
                    lenX = -1;
                } else {
                    if (xSign < 0) {
                        restX =   - residualX;
                    } else {
                        restX = 1 - residualX;
                    }
                    lenX = restX / UnitVector.Item1;
                }
                double restY;
                double lenY;
                if (ySign == 0) {
                    lenY = -1;
                } else {
                    if (ySign < 0) {
                        restY =   - residualY;
                    } else {
                        restY = 1 - residualY;
                    }
                    lenY = restY / UnitVector.Item2;
                }
            }
        }
    }
}
