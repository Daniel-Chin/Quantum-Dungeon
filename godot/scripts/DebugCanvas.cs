using Godot;
using System;
using System.Collections.Generic;

public class DebugCanvas : Node2D {
    public static DebugCanvas Self = null;
    public List<Point> PolygonVerts;
    public Vision.RBTree MyRBTree;
    public HashSet<PointInt> GridPoints;
    public Dictionary<PointInt, Vision.Connections> Edges;
    public override void _Ready() {
        Self = this;
    }
    public override void _Draw() {
        PlotVertices();
        PlotLineSegs();
        PlotGraph();
    }
    void PlotVertices() {
        if (PolygonVerts == null) return;
        // GD.PrintS("PlotVer");
        Vector2 last = Display.Coord2Pixel(GameState.PlayerPos.Offset05());
        float i = 0;
        float step = 1f / PolygonVerts.Count;
        foreach (Point p in PolygonVerts) {
            i += step;
            Vector2 next = Display.Coord2Pixel(p);
            DrawLine(
                next, last, 
                new Color(i, 0, 1), 
                width: 2
            );
            last = next;
        }
    }
    void PlotLineSegs() {
        if (MyRBTree == null) return;
        float i = 0;
        float step = 1f / MyRBTree.Count;
        foreach (LineSegment lS in MyRBTree.Keys) {
            i += step;
            DrawLine(
                Display.Coord2Pixel(lS.Start), 
                Display.Coord2Pixel(lS.End), 
                new Color(i, 1, 0), 
                width: 2
            );
        }
    }
    void PlotGraph() {
        if (GridPoints == null) return;
        foreach (KeyValuePair<PointInt, Vision.Connections> e in Edges) {
            int x = e.Key.IntX;
            int y = e.Key.IntY;
            Vision.Connections con = e.Value;
            if (con.XPos) {
                DrawLine(
                    Display.Coord2Pixel(new Point(
                        x, y
                    )), 
                    Display.Coord2Pixel(new Point(
                        x + .3, y
                    )), 
                    new Color(1, 1, 0), 
                    width: 2
                );
            }
            if (con.XNeg) {
                DrawLine(
                    Display.Coord2Pixel(new Point(
                        x, y
                    )), 
                    Display.Coord2Pixel(new Point(
                        x - .3, y
                    )), 
                    new Color(1, 1, 0), 
                    width: 2
                );
            }
            if (con.YPos) {
                DrawLine(
                    Display.Coord2Pixel(new Point(
                        x, y
                    )), 
                    Display.Coord2Pixel(new Point(
                        x, y + .3
                    )), 
                    new Color(1, 1, 0), 
                    width: 2
                );
            }
            if (con.YNeg) {
                DrawLine(
                    Display.Coord2Pixel(new Point(
                        x, y
                    )), 
                    Display.Coord2Pixel(new Point(
                        x, y - .3
                    )), 
                    new Color(1, 1, 0), 
                    width: 2
                );
            }
        }
    }
}
