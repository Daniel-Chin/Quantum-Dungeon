using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class Vision {
    class RBTree : SortedDictionary<LineSegmentInt, bool> {
        protected Point EyePos;
        public LineSegmentInt Memory {
            get; protected set;
        }
        public LineSegmentInt Min() => Keys.First();
        public LineSegmentInt Max() => Keys.Last();
        public RBTree(Point eyePos) {
            EyePos = eyePos;
        }
        public override string ToString() {
            StringBuilder sB = new StringBuilder();
            sB.Append("RBTree [ \n");
            foreach (LineSegmentInt lineSeg in Keys) {
                sB.Append("  ");
                sB.Append(lineSeg);
                sB.Append('\n');
            }
            sB.Append("]");
            return sB.ToString();
        }
        public void Memorize() {
            Memory = Min();
        }
        public new bool ContainsKey(LineSegmentInt lS) {
            lS.UpdateManhattanMag(EyePos);
            return base.ContainsKey(lS);
        }
        public new void Add(LineSegmentInt lS, bool b) {
            lS.UpdateManhattanMag(EyePos);
            // GD.PrintS(lS);
            base.Add(lS, b);
        }
        public void Initialize(
            IEnumerable<PointInt> points, 
            Dictionary<PointInt, Connections> edges
        ) {
            foreach (PointInt p in points) {
                int x = (int) Math.Ceiling(EyePos.X);
                int y = (int) Math.Ceiling(EyePos.Y);
                if (p.IntY == y && p.IntX < x) {
                    if (edges[p][0, 1]) {
                        LineSegmentInt lineSeg = new LineSegmentInt(
                            p, new PointInt(p.IntX, p.IntY - 1)
                        );
                        // GD.Print();
                        // GD.PrintS("Adding", lineSeg);
                        // GD.PrintS("Into", this);
                        // GD.PrintS("Comparisons {");
                        Add(lineSeg, true);
                        // GD.PrintS("}");
                    }
                }
            }
        }
    }
    protected class Connections {
        bool XPos;
        bool YPos;
        bool XNeg;
        bool YNeg;
        public bool this[int x, int y] {
            get {
                if (x == -1) return XNeg;
                if (y == -1) return YNeg;
                if (x == +1) return XPos;
                if (y == +1) return YPos;
                throw new IndexOutOfRangeException();
            }
            set {
                if        (x == -1) {
                    XNeg = value;
                } else if (y == -1) {
                    YNeg = value;
                } else if (x == +1) {
                    XPos = value;
                } else if (y == +1) {
                    YPos = value;
                } else throw new IndexOutOfRangeException();
            }
        }
    }
    protected static void MapToGraph(
        Map map, HashSet<PointInt> points, 
        Dictionary<PointInt, Connections> edges
    ) {
        foreach (KeyValuePair<PointInt, EnumClass> cell in map) {
            if (cell.Value is Tile tile) {
                if (tile.DoesBlock()) {
                    PointInt p00 = cell.Key;
                    for (int dx = -1; dx <= 1; dx += 2) {
                        for (int dy = -1; dy <= 1; dy += 2) {
                            PointInt corner = new PointInt(
                                p00.IntX + dx, p00.IntY + dy
                            );
                            points.Add(corner);
                            if (! edges.ContainsKey(corner)) {
                                edges.Add(corner, new Connections());
                            }
                            Connections connections = edges[corner];
                            connections[dx, 0] = ! connections[dx, 0];
                            connections[0, dy] = ! connections[0, dy];
                        }
                    }
                }
            } else {
                throw new Exception("9g835h4");
            }
        }
    }
    protected class PointPolar : IComparable {
        public PointInt ThePoint;
        public double Phase;
        public double Manhattan;
        int IComparable.CompareTo(object obj) {
            if (obj is PointPolar other) {
                if (Math.Abs(Phase - other.Phase) < double.Epsilon) {
                    return Manhattan.CompareTo(other.Manhattan);
                } else {
                    return Phase    .CompareTo(other.Phase);
                }
            } else {
                throw new ArgumentException();
            }
        }
    }
    protected static IEnumerable<PointInt> SortByOrientation(
        HashSet<PointInt> points, Point eyePos
    ) {
        PointPolar[] array = new PointPolar[points.Count];
        int i = 0;
        foreach (PointInt p in points) {
            double dY = p.Y - eyePos.Y;
            double dX = p.X - eyePos.X;
            array[i ++] = new PointPolar() {
                ThePoint = p, 
                Phase = Math.Atan2(dY, dX), 
                Manhattan = Math.Abs(dX) + Math.Abs(dY),
            };
        }
        Array.Sort(array);
        return array.Select(x => x.ThePoint);
    }
    public static List<Point> GetVertices(
        Map map, PointInt playerPos
    ) {
        Point eyePos = playerPos.Offset05();
        List<Point> vertices = new List<Point>();
        RBTree rBTree = new RBTree(eyePos);
        HashSet<PointInt> gridPoints = new HashSet<PointInt>();
        Dictionary<PointInt, Connections> edges = new Dictionary<PointInt, Connections>();
        MapToGraph(map, gridPoints, edges);
        IEnumerable<PointInt> sortedGridPoints = SortByOrientation(
            gridPoints, eyePos
        );
        rBTree.Initialize(sortedGridPoints, edges);
        foreach (PointInt point in sortedGridPoints) {
            bool isVertexSeen = false;
            if (point.ManhattanMag() - 1 < rBTree.Min().ManhattanMag) {
                isVertexSeen = true;
                rBTree.Memorize();
            }
            int deltaSeenEdges = 0;
            for (int x = -1; x <= 1; x += 2) {
                for (int y = -1; y <= 1; y += 2) {
                    if (edges[point][x, y]) {
                        LineSegmentInt lineSeg = new LineSegmentInt(
                            point, new PointInt(
                                point.IntX + x, 
                                point.IntY + y
                            )
                        );
                        // GD.PrintS("ContainsKey");
                        // GD.PrintS(rBTree);
                        // GD.PrintS(lineSeg);
                        // GD.PrintS(rBTree.ContainsKey(lineSeg));
                        if (rBTree.ContainsKey(lineSeg)) {
                            rBTree.Remove(lineSeg);
                            if (isVertexSeen) {
                                deltaSeenEdges --;
                            }
                        } else {
                            // GD.PrintS("adding...");
                            rBTree.Add(lineSeg, true);
                            // GD.PrintS("added.");
                            if (isVertexSeen) {
                                deltaSeenEdges ++;
                            }
                        }
                    }
                }
            }
            if (isVertexSeen) {
                if (deltaSeenEdges == 0) {
                    vertices.Add(point);
                } else {
                    LineSegment backWall;
                    if (deltaSeenEdges == 2) {
                        backWall = rBTree.Memory;
                    } else {
                        backWall = rBTree.Min();
                    }
                    Point intersect = backWall.IntersectWith(new LineSegment(
                        playerPos.Offset05(), point
                    ));
                    if (deltaSeenEdges == 2) {
                        vertices.Add(intersect);
                        vertices.Add(point);
                    } else {
                        vertices.Add(point);
                        vertices.Add(intersect);
                    }
                }
            }
        }
        return vertices;
    }
}
