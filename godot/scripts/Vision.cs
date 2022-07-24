using Godot;
using System;
using System.Collections.Generic;

public class Vision {
    private class Point {
        public int X;
        public int Y;
        private double? Rad = null;
        public double GetRad() {
            if (Rad is double valRad) {
                return valRad;
            } else {
                Rad = Math.Atan2(Y, X);
                return (double) Rad;
            }
        }
    }
    private interface Arc {
        double MidRad();
    }
    private class FullArc : Arc {
        public double MidRad() {
            return 0;
        }
    }
    private class NiceArc : Arc {
        // Cannot be an empty arc
        public Point Start;
        public Point End;
        public double MidRad() {
            return (Start.GetRad() + End.GetRad()) / 2;
        }
    }
    enum Label {
        Seen, Blocked, 
    }
    public static void See(
        Dictionary<Tuple<int, int>, int> world
    ) {
        Dictionary<
            Tuple<int, int>, Label
        > labels = new Dictionary<Tuple<int, int>, Label>();
        List<Arc> residualArcs = new List<Arc>();
        residualArcs.Add(new FullArc());
        while (residualArcs.Count != 0) {
            Arc arc = residualArcs[0];
            CastRay(world, labels, arc.MidRad());
        }
    }
    private static void CastRay(
        Dictionary<Tuple<int, int>, int> world, 
        Dictionary<Tuple<int, int>, Label> labels, 
        double rad
    ) {

    }
}
