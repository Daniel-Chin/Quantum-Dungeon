using Godot;
using System;
using System.Collections.Generic;

public class Vision {
    static readonly double TWO_PI = 2 * Math.PI;
    public enum C {
        Less, Equal, Great, 
    }
    public static C Compare(int a, int b) {
        if (a == b) {
            return C.Equal;
        } else if (a < b) {
            return C.Less;
        } else return C.Great;
    }
    enum Quadrant {
        A, B, C, D, XPos, XNeg, YPos, YNeg, Origin, 
    }
    enum Draquant {
        E, N, W, S, A, B, C, D, Origin, 
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
            if (X + Y == 0) {
                if (X - Y == 0) {
                    return Draquant.Origin;
                } else if (X - Y < 0) {
                    return Draquant.B;
                } else {
                    return Draquant.D;
                }
            } else if (X + Y < 0) {
                if (X - Y == 0) {
                    return Draquant.C;
                } else if (X - Y < 0) {
                    return Draquant.W;
                } else {
                    return Draquant.S;
                }
            } else {
                if (X - Y == 0) {
                    return Draquant.A;
                } else if (X - Y < 0) {
                    return Draquant.N;
                } else {
                    return Draquant.E;
                }
            }
        }
        public C Compare(Orientation that) {
            int result = Y * that.X - X * that.Y;
            if (result == 0) {
                return C.Equal;
            } else if (result < 0) {
                return C.Less;
            } else {
                return C.Great;
            }
        }
        public void Simplify() {
            int gcd = Global.GCD(X, Y);
            X /= gcd;
            Y /= gcd;
        }
    }
    private class Arc {
        // Can be neither 0 nor 2 * PI. 
        public Orientation Start;
        public Orientation End;
        public void ExpandTo(Orientation orientation) {
            if (orientation.Compare(Start) == C.Less) {
                Start = orientation;
            }
            if (orientation.Compare(End)   == C.Great) {
                End   = orientation;
            }
        }
        public bool DoesContain(Orientation orientation) {
            double start = Math.Atan2(Start.Y, Start.X);
            double end   = Math.Atan2(End  .Y, End  .X);
            double test  = Math.Atan2(orientation.Y, orientation.X);
            double start_test = (test - start) % TWO_PI;
            double  test_end  = (end  - test ) % TWO_PI;
            double start_end  = (end  - start) % TWO_PI;
            return start_test + test_end - start_end < Math.PI;
        }
    }
    private class ArcSet {
        bool IsFull;
        private Queue<Arc> Arcs;
        public ArcSet() {
            IsFull = true;
            Arcs = new Queue<Arc>();
        }
        public bool IsEmpty() {
            return ! IsFull && Arcs.Count == 0;
        }
        public Orientation Sample() {
            if (IsFull) {
                return new Orientation() {X=1, Y=0};
            } else {
                Arc arc = Arcs.Peek();
                Orientation start = arc.Start;
                Orientation end   = arc.End;
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
        public void Subtract(Arc hole) {
            hole.Start.Simplify();
            hole.End  .Simplify();
            if (IsFull) {
                IsFull = false;
                Arcs.Enqueue(new Arc() {
                    Start = hole.End, 
                    End   = hole.Start, 
                });
            } else {
                // Safe to assume that only 
                // the first arc is affected
                Arc arc = Arcs.Dequeue();
                bool aChS = arc .DoesContain(hole.Start);
                bool aChE = arc .DoesContain(hole.End);
                bool hCaS = hole.DoesContain(arc .Start);
                bool hCaE = hole.DoesContain(arc .End);
                if (aChS) {
                    if (aChE) {
                        if (hCaE) {
                            Arcs.Enqueue(new Arc() {
                                Start = hole.End, 
                                End = hole.Start, 
                            });
                        } else {
                            Arcs.Enqueue(new Arc() {
                                Start = arc.Start, 
                                End = hole.Start, 
                            });
                            Arcs.Enqueue(new Arc() {
                                Start = hole.End, 
                                End = arc.End, 
                            });
                        }
                    } else {
                        Arcs.Enqueue(new Arc() {
                            Start = arc.Start, 
                            End = hole.Start, 
                        });
                    }
                } else {
                    if (aChE) {
                        Arcs.Enqueue(new Arc() {
                            Start = hole.End, 
                            End = arc.End, 
                        });
                    } else {
                        if (hCaS) {
                            // hole covers arc
                        } else {
                            Arcs.Enqueue(arc);
                        }
                    }
                }
            }
        }
    }
    public static Dictionary<Tuple<int, int>, bool> See(
        Tuple<int, int> playerPos, 
        Dictionary<Tuple<int, int>, int> world
    ) {
        Dictionary<
            Tuple<int, int>, bool
        > isSeen = new Dictionary<Tuple<int, int>, bool>();
        ArcSet residualArcs = new ArcSet();
        // double eyeX = playerPos.Item1 + .5;
        // double eyeY = playerPos.Item2 + .5;
        while (! residualArcs.IsEmpty()) {
            Orientation orientation = residualArcs.Sample();
            var (collideX, collideY) = CastRay(
                playerPos, 
                orientation, world, isSeen
            );
            var (relX, relY) = Offset(collideX, collideY, playerPos);
            Assert(! (relX == 0 && relY == 0));
            Arc newArc = new Arc();
            newArc.Start = new Orientation() {
                X = relX, 
                Y = relY, 
            };
            newArc.End = newArc.Start;
            newArc.ExpandTo(new Orientation() {
                X = relX + 2, 
                Y = relY, 
            });
            newArc.ExpandTo(new Orientation() {
                X = relX, 
                Y = relY + 2, 
            });
            newArc.ExpandTo(new Orientation() {
                X = relX + 2, 
                Y = relY + 2, 
            });
            residualArcs.Subtract(newArc);
        }
        return isSeen;
    }
    private static Tuple<int, int> CastRay(
        Tuple<int, int> playerPos, 
        // double eyeX, double eyeY, 
        Orientation orientation, 
        Dictionary<Tuple<int, int>, int> world, 
        Dictionary<Tuple<int, int>, bool> labels
    ) {
        var (cellX, cellY) = playerPos;
        bool atEye = true;
        while (true) {
            {
                Tuple<int, int> cellXY = Tuple.Create(
                    cellX, cellY
                );
                labels[cellXY] = true;
                if (Tile.DoesBlock(world[cellXY])) {
                    break;
                }
            }
            if (atEye) {
                switch (orientation.GetDraquant()) {
                    case Draquant.E:
                        cellX ++;
                        break;
                    case Draquant.N:
                        cellY ++;
                        break;
                    case Draquant.W:
                        cellX --;
                        break;
                    case Draquant.S:
                        cellY --;
                        break;
                    case Draquant.A:
                        cellX ++;
                        cellY ++;
                        break;
                    case Draquant.B:
                        cellX --;
                        cellY ++;
                        break;
                    case Draquant.C:
                        cellX --;
                        cellY --;
                        break;
                    case Draquant.D:
                        cellX ++;
                        cellY --;
                        break;
                    default:
                        throw new Exception("gbwr3i7h");
                }
                atEye = false;
            } else {
                var (relX, relY) = Offset(cellX, cellY, playerPos);
                C c00 = orientation.Compare(new Orientation() {
                    X = relX, 
                    Y = relY, 
                });
                C c02 = orientation.Compare(new Orientation() {
                    X = relX, 
                    Y = relY + 2, 
                });
                C c20 = orientation.Compare(new Orientation() {
                    X = relX + 2, 
                    Y = relY, 
                });
                C c22 = orientation.Compare(new Orientation() {
                    X = relX + 2, 
                    Y = relY + 2, 
                });
                switch (orientation.GetQuadrant()) {
                    case Quadrant.A:
                        Assert(c20 == C.Great);
                        Assert(c02 == C.Less);
                        switch (c22) {
                            case C.Equal:
                                cellX ++;
                                cellY ++;
                                break;
                            case C.Less:
                                cellX ++;
                                break;
                            case C.Great:
                                cellY ++;
                                break;
                        }
                        break;
                    case Quadrant.B:
                        Assert(c22 == C.Great);
                        Assert(c00 == C.Less);
                        switch (c02) {
                            case C.Equal:
                                cellX --;
                                cellY ++;
                                break;
                            case C.Less:
                                cellY ++;
                                break;
                            case C.Great:
                                cellX --;
                                break;
                        }
                        break;
                    case Quadrant.C:
                        Assert(c02 == C.Great);
                        Assert(c20 == C.Less);
                        switch (c00) {
                            case C.Equal:
                                cellX --;
                                cellY --;
                                break;
                            case C.Less:
                                cellX --;
                                break;
                            case C.Great:
                                cellY --;
                                break;
                        }
                        break;
                    case Quadrant.D:
                        Assert(c00 == C.Great);
                        Assert(c22 == C.Less);
                        switch (c20) {
                            case C.Equal:
                                cellX ++;
                                cellY --;
                                break;
                            case C.Less:
                                cellY --;
                                break;
                            case C.Great:
                                cellX ++;
                                break;
                        }
                        break;
                    case Quadrant.XPos:
                        cellX ++;
                        break;
                    case Quadrant.YPos:
                        cellY ++;
                        break;
                    case Quadrant.XNeg:
                        cellX --;
                        break;
                    case Quadrant.YNeg:
                        cellY --;
                        break;
                }
            }
        }
        return Tuple.Create(cellX, cellY);
    }
    private static Tuple<int, int> Offset(
        int x, int y, 
        Tuple<int, int> playerPos
    ) {
        return new Tuple<int, int>(
            2 * (x - playerPos.Item1) - 1, 
            2 * (y - playerPos.Item2) - 1
        );
    }
    public static void Assert(bool x) {
        if (! x) {
            throw new Exception("Assertion failed");
        }
    }
}
