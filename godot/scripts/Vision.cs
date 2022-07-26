using System;
using System.Collections.Generic;
using System.Linq;

public class Vision {
    class RBTree : SortedDictionary<LineSegment, bool> {
        public LineSegment Memory {
            get; protected set;
        }
        public LineSegment Min() => Keys.First();
        public LineSegment Max() => Keys.Last();
        public void Memorize() {
            Memory = Min();
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
        Map map, HashSet<Point> points, 
        Dictionary<Point, Connections> edges
    ) {
        foreach (KeyValuePair<Point, EnumClass> cell in map) {
            if (cell.Value is Tile tile) {
                if (tile.DoesBlock()) {
                    Point p00 = cell.Key;
                    for (int dx = -1; dx <= 1; dx += 2) {
                        for (int dy = -1; dy <= 1; dy += 2) {
                            Point corner = new Point(
                                p00.X + dx, p00.Y + dy
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
        public Point ThePoint;
        public double Phase;
        public double Manhattan;
        public int CompareTo(object obj) {
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
    protected static IEnumerable<Point> SortByOrientation(
        HashSet<Point> points, Point playerPos
    ) {
        Point eye = playerPos.Offset05();
        List<PointPolar> list = new List<PointPolar>();
        foreach (Point p in points) {
            double dY = p.Y - eye.Y;
            double dX = p.X - eye.X;
            list.Add(new PointPolar() {
                ThePoint = p, 
                Phase = Math.Atan2(dY, dX), 
                Manhattan = Math.Abs(dX) + Math.Abs(dY),
            });
        }
        list.Sort();
        return list.Select(x => x.ThePoint);
    }
    public static List<Point> GetVertices(
        Map map, Point playerPos
    ) {
        List<Point> vertices = new List<Point>();
        RBTree rBTree = new RBTree();
        HashSet<Point> gridPoints = new HashSet<Point>();
        Dictionary<Point, Connections> edges = new Dictionary<Point, Connections>();
        MapToGraph(map, gridPoints, edges);
        IEnumerable<Point> sortedGridPoints = SortByOrientation(
            gridPoints, playerPos
        );
        // Missing init condition
        foreach (Point point in sortedGridPoints) {
            int deltaSeenEdges = 0;
            bool isVertexSeen = false;
            rBTree.Memorize();
            for (int x = -1; x <= 1; x += 2) {
                for (int y = -1; y <= 1; y += 2) {
                    if (edges[point][x, y]) {
                        LineSegment lineSeg = new LineSegment(
                            point, new Point(
                                point.X + x, 
                                point.Y + y
                            )
                        );
                        if (rBTree.ContainsKey(lineSeg)) {
                            if (lineSeg == rBTree.Min()) {
                                isVertexSeen = true;
                                deltaSeenEdges --;
                            }
                            rBTree.Remove(lineSeg);
                        } else {
                            lineSeg.UpdateManhattanMag(playerPos);
                            rBTree.Add(lineSeg, true);
                            if (lineSeg == rBTree.Min()) {
                                isVertexSeen = true;
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
