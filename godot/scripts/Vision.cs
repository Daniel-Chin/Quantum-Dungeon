using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class Vision {
    public class RBTree : SortedDictionary<LineSegmentInt, bool> {
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
                    if (edges[p][0, -1]) {
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
    public class Connections {
        public bool XPos;
        public bool YPos;
        public bool XNeg;
        public bool YNeg;
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
        map.WrapWithUnknown();
        foreach (KeyValuePair<PointInt, EnumClass> cell in map) {
            Tile tile = (Tile) cell.Value;
            if (tile.DoesBlockVision()) {
                PointInt p00 = cell.Key;
                for (int dx = 0; dx < 2; dx ++) {
                    for (int dy = 0; dy < 2; dy ++) {
                        PointInt corner = new PointInt(
                            p00.IntX + dx, p00.IntY + dy
                        );
                        points.Add(corner);
                        int dirX = 1 - dx * 2;
                        int dirY = 1 - dy * 2;
                        if (! edges.ContainsKey(corner)) {
                            edges.Add(corner, new Connections());
                        }
                        Connections connections = edges[corner];
                        connections[dirX, 0] = ! connections[dirX, 0];
                        connections[0, dirY] = ! connections[0, dirY];
                    }
                }
            }
        }
    }
    protected class PointPolar : IComparable<PointPolar> {
        public PointInt ThePoint;
        public double Phase;
        public double Manhattan;
        int IComparable<PointPolar>.CompareTo(PointPolar that) {
            if (Math.Abs(Phase - that.Phase) < double.Epsilon) {
                return Manhattan.CompareTo(that.Manhattan);
            } else {
                return Phase    .CompareTo(that.Phase);
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
    // public static int DEBUG_I = 0;
    public static List<Point> GetVertices(
        Map map, Point eyePos
    ) {
        List<Point> vertices = new List<Point>();
        RBTree rBTree = new RBTree(eyePos);
        HashSet<PointInt> gridPoints = new HashSet<PointInt>();
        Dictionary<PointInt, Connections> edges = new Dictionary<PointInt, Connections>();
        MapToGraph(map, gridPoints, edges);
        // DebugCanvas.Self.GridPoints = gridPoints;
        // DebugCanvas.Self.Edges      = edges;
        // DebugCanvas.Self.PolygonVerts = gridPoints.Select(x => (Point)x).ToList();
        IEnumerable<PointInt> sortedGridPoints = SortByOrientation(
            gridPoints, eyePos
        );
        // DebugCanvas.Self.PolygonVerts = sortedGridPoints.Select(x => (Point)x).ToList();
        rBTree.Initialize(sortedGridPoints, edges);
        // DebugCanvas.Self.MyRBTree = rBTree;
        // DebugCanvas.Self.PolygonVerts = vertices;
        // if (DEBUG_I == 0)
        //     return new List<Point>();
        // int debugI = 0;
        foreach (PointInt point in sortedGridPoints) {
            bool isVertexSeen = false;
            if (
                point.ManhattanMag(eyePos) - 1 
                < rBTree.Min().ManhattanMag
            ) {
                isVertexSeen = true;
                rBTree.Memorize();
            }
            int deltaSeenEdges = 0;
            for (int i = 0; i < 4; i ++) {
                int x;
                int y;
                switch (i) {
                    case 0: x = -1; y =  0; break;
                    case 1: x =  0; y = -1; break;
                    case 2: x = +1; y =  0; break;
                    case 3: x =  0; y = +1; break;
                    default: throw new Exception("Impo");
                }
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
            if (isVertexSeen) {
                // if (DEBUG_I == ++ debugI) return new List<Point>();
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
                        eyePos, point
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
        List<Point> processed = new List<Point>();
        double lastPhase = - Math.PI;
        Point lastPoint = null;
        foreach (Point p in vertices) {
            double phase = Math.Atan2(
                p.Y - eyePos.Y, 
                p.X - eyePos.X
            );
            if (Math.Abs(phase - lastPhase) > double.Epsilon) {
                if (lastPoint != null) {
                    if (lastPoint != processed.Last()) {
                        processed.Add(lastPoint);
                    }
                    lastPoint = null;
                }
                processed.Add(p);
            } else {
                lastPoint = p;
            }
            lastPhase = phase;
        }
        if (lastPoint != null) {
            if (lastPoint != processed.Last()) {
                processed.Add(lastPoint);
            }
        }
        return processed;
    }
}
