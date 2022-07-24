using Godot;
using System;
using System.Collections.Generic;

public class Vision {
    private class Point {
        public int X;
        public int Y;
        private double? Rad = null;
        public double GetRad(double eyeX, double eyeY) {
            if (Rad is double valRad) {
                return valRad;
            } else {
                Rad = Math.Atan2(Y - eyeY, X - eyeX);
                return (double) Rad;
            }
        }
    }
    private interface Arc {
        double MidRad(double eyeX, double eyeY);
    }
    private class FullArc : Arc {
        public double MidRad(double eyeX, double eyeY) {
            return 0;
        }
    }
    private class NiceArc : Arc {
        // Cannot be an empty arc
        public Point Start;
        public Point End;
        public double MidRad(double eyeX, double eyeY) {
            double x = Global.random.NextDouble();
            return (
                Start.GetRad(eyeX, eyeY) * x
                + End.GetRad(eyeX, eyeY) * (1 - x)
            );
        }
    }
    enum Label {
        Seen, Blocked, 
    }
    public static void See(
        Tuple<int, int> playerPos, 
        Dictionary<Tuple<int, int>, int> world
    ) {
        Dictionary<
            Tuple<int, int>, Label
        > labels = new Dictionary<Tuple<int, int>, Label>();
        List<Arc> residualArcs = new List<Arc>();
        residualArcs.Add(new FullArc());
        double eyeX = playerPos.Item1 + .5;
        double eyeY = playerPos.Item2 + .5;
        labels[playerPos] = Label.Seen;
        while (residualArcs.Count != 0) {
            Arc arc = residualArcs[0];
            CastRay(
                playerPos, eyeX, eyeY, 
                arc.MidRad(eyeX, eyeY), world, labels
            );
        }
    }
    private static readonly double SW = - .75 * Math.PI;
    private static readonly double SE = - .25 * Math.PI;
    private static readonly double NE = + .25 * Math.PI;
    private static readonly double NW = + .75 * Math.PI;
    private static void CastRay(
        Tuple<int, int> playerPos, 
        double eyeX, double eyeY, double rad, 
        Dictionary<Tuple<int, int>, int> world, 
        Dictionary<Tuple<int, int>, Label> labels
    ) {
        Tuple<int, int> cellXY = new Tuple<int, int>(
            playerPos.Item1, 
            playerPos.Item2
        );
        bool atEye = true;
        while (! Tile.DoesBlock(world[cellXY])) {
            var (cellX, cellY) = cellXY;
            if (atEye) {
                if (SW < rad && rad <= SE) {
                    cellXY = Tuple.Create(cellX, cellY - 1);
                } else if (SE < rad && rad <= NE) {
                    cellXY = Tuple.Create(cellX + 1, cellY);
                } else if (NE < rad && rad <= NW) {
                    cellXY = Tuple.Create(cellX, cellY + 1);
                } else {
                    cellXY = Tuple.Create(cellX - 1, cellY);
                }
                atEye = false;
            } else {
                double relX = cellX - eyeX;
                double relY = cellY - eyeY;
                bool doFlip = relX < 0;
                bool r00 = Math.Atan2(relX, relY) < rad;
                bool r10 = Math.Atan2(relX + 1, relY) < rad;
            }
            labels[cellXY] = Label.Seen;
            new Fraction
        }
    }
}
