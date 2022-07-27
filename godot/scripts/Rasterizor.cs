using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Rasterizor : Viewport {
    Polygon2D PolygonRaster;
    Camera2D CameraRaster;
    public override void _Ready() {
        PolygonRaster = GetNode<Polygon2D>("PolygonRaster");
        CameraRaster  = GetNode<Camera2D>("CameraRaster");
    }
    public void Eat(
        List<Point> vertices, Dictionary<PointInt, bool> output
    ) { 
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        PolygonRaster.Polygon = new Vector2[vertices.Count];
        int i = 0;
        foreach (Point p in vertices) {
            minX = (int) Math.Min(minX, Math.Floor  (p.X));
            minY = (int) Math.Min(minY, Math.Floor  (p.Y));
            maxX = (int) Math.Max(maxX, Math.Ceiling(p.X));
            maxY = (int) Math.Max(maxY, Math.Ceiling(p.Y));
            PolygonRaster.Polygon[i] = new Vector2(
                (float) p.X, (float) p.Y
            );
            i ++;
        }
        Size = new Vector2(
            maxX - minX + 2, 
            maxY - minY + 2 
        );
        CameraRaster.Offset = new Vector2(minX, minY);
        PolygonRaster.Update();
        GD.PrintS(minX, maxX, minY, maxY);
        Image img = GetTexture().GetData();
        img.Lock();
        img.SavePng("temp.png");
        for (int x = 0; x < Size.x; x ++) {
            for (int y = 0; y < Size.y; y ++) {
                if (img.GetPixel(x, y).r != 0) {
                    output.Add(new PointInt(
                        x + minX, y + minY
                    ), true);
                }
            }
        }
    }
}
