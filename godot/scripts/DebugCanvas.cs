using Godot;
using System;
using System.Collections.Generic;

public class DebugCanvas : Node2D {
    public static DebugCanvas Self = null;
    public List<Point> CachedVertices;
    public override void _Ready() {
        Self = this;
    }
    public override void _Draw() {
        PlotVertices();
    }
    void PlotVertices() {
        if (CachedVertices == null) return;
        // GD.PrintS("PlotVer");
        Vector2 last = Display.Coord2Pixel(GameState.PlayerPos.Offset05());
        float i = 0;
        float step = 1f / CachedVertices.Count;
        foreach (Point p in CachedVertices) {
            i += step;
            Vector2 next = Display.Coord2Pixel(p);
            DrawLine(
                next, last, 
                new Color(i, 1, 0), 
                width: 2
            );
            last = next;
        }
    }
}
