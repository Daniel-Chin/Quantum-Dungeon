using System;
using System.Collections.Generic;
using System.Linq;

public class Vision {
    class RBTree : SortedDictionary<LineSegment<int>, bool> {
        public LineSegment<int> Memory {
            get; protected set;
        }
        public LineSegment<int> Min() => Keys.First();
        public LineSegment<int> Max() => Keys.Last();
        public void Memorize() {
            Memory = Min();
        }
    }
    class Connections {
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
    public static List<Point> GetVertices(
        Map<Tile> map, Tuple<int, int> playerPos
    ) {
        List<Point> vertices = new List<Point>();
        RBTree rBTree = new RBTree();
        List<Point<int>> gridPoints = new List<Point<int>>();
        Dictionary<Point<int>, Connections> edges = new Dictionary<Point<int>, Connections>();
        foreach (Point<int> point in gridPoints) {
            int deltaSeenEdges = 0;
            bool isVertexSeen = false;
            rBTree.Memorize();
            for (int x = -1; x <= 1; x += 2) {
                for (int y = -1; y <= 1; y += 2) {
                    if (edges[point][x, y]) {
                        LineSegment<int> lineSeg = new LineSegment<int>(
                            point, new Point<int>(
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
                vertices.Add(point);
            }
            if (deltaSeenEdges != 0) {
                LineSegment<int> backWall;
                if (deltaSeenEdges == 2) {
                    backWall = rBTree.Memory;
                } else {
                    backWall = rBTree.Min();
                }
            }
        }
    }
}
