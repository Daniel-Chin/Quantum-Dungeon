using Godot;
using System;
using System.Collections.Generic;

public class Vision {
    enum Quadrant {
        A, B, C, D, XPos, XNeg, YPos, YNeg, Origin, 
    }
    enum Draquant {
        E, N, W, S, 
    }
    private class Orientation {
        // Cannot be (0, 0)
        public int X;
        public int Y;
        public Quadrant GetQuadrant() {
            if (X == 0) {
                if (Y == 0) {
                    return Quadrant.Origin;
                } else if (Y < 0) {
                    return Quadrant.YNeg;
                } else {
                    return Quadrant.YPos;
                }
            } else if (X < 0) {
                if (Y == 0) {
                    return Quadrant.XNeg;
                } else if (Y < 0) {
                    return Quadrant.C;
                } else {
                    return Quadrant.B;
                }
            } else {
                if (Y == 0) {
                    return Quadrant.XPos;
                } else if (Y < 0) {
                    return Quadrant.D;
                } else {
                    return Quadrant.A;
                }
            }
        }
        public Draquant GetDraquant() {
            if (X + Y < 0) {
                if (X - Y < 0) {
                    return Draquant.W;
                } else {
                    return Draquant.S;
                }
            } else {
                if (X - Y < 0) {
                    return Draquant.N;
                } else {
                    return Draquant.E;
                }
            }
        }
        // public enum C {
        //     LESS, EQUAL, GREAT
        // }
        // public C Compare(Orientation that) {
        //     int result = X * that.Y - Y * that.X;
        //     if (result == 0) {
        //         return C.EQUAL;
        //     } else if (result < 0) {
        //         return C.LESS;
        //     } else {
        //         return C.GREAT;
        //     }
        // }
    }
    private class Arc {
        // Can be neither 0 nor 2 * PI. 
        public Orientation Start;
        public Orientation End;
    }
    private class ArcSet {
        bool IsFull;
        public List<Arc> Arcs;
        public ArcSet() {
            IsFull = true;
            Arcs = new List<Arc>();
        }
        public bool IsEmpty() {
            return ! IsFull && Arcs.Count == 0;
        }
        public Orientation Sample() {
            if (IsFull) {
                return new Orientation() {X=1, Y=0};
            } else {
                Orientation start = Arcs[0].Start;
                Orientation end   = Arcs[0].End;
                if (
                    start.X * end.Y <= 
                    start.Y * end.X
                ) {
                    start = new Orientation() {
                        X = - start.Y, 
                        Y = + start.X, 
                    };
                    end   = new Orientation() {
                        X = +   end.Y, 
                        Y = -   end.X, 
                    };
                }
                return new Orientation() {
                    X = start.X + end.X, 
                    Y = start.Y + end.Y, 
                };
            }
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
        ArcSet residualArcs = new ArcSet();
        // double eyeX = playerPos.Item1 + .5;
        // double eyeY = playerPos.Item2 + .5;
        labels[playerPos] = Label.Seen;
        while (! residualArcs.IsEmpty()) {
            CastRay(
                playerPos, 
                residualArcs.Sample(), world, labels
            );
        }
    }
    private static void CastRay(
        Tuple<int, int> playerPos, 
        // double eyeX, double eyeY, 
        Orientation orientation, 
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
                switch (orientation.GetDraquant()) {
                    case Draquant.E:
                        cellXY = Tuple.Create(cellX + 1, cellY);
                        break;
                    case Draquant.N:
                        cellXY = Tuple.Create(cellX, cellY + 1);
                        break;
                    case Draquant.W:
                        cellXY = Tuple.Create(cellX - 1, cellY);
                        break;
                    case Draquant.S:
                        cellXY = Tuple.Create(cellX, cellY - 1);
                        break;
                }
                atEye = false;
            } else {
                double relX = 2 * (cellX - playerPos.Item1) - 1;
                double relY = 2 * (cellY - playerPos.Item2) - 1;
                
            }
            labels[cellXY] = Label.Seen;
        }
    }
}
