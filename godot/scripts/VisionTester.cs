using Godot;
using System;
using System.Collections.Generic;

public class VisionTester : GamePlay {
    Map TheReal;
    public VisionTester(Main main): base(main) {
        Reset();
    }
    public override void Reset() {
        GameState.Reset();
        TheReal = new Map();
        for (int i = -10; i < 10; i ++) {
            for (int j = -10; j < 10; j ++) {
                Tile tile;
                if (Global.random.Next() % 100 < 30) {
                    tile = Tile.WALL;
                } else {
                    tile = Tile.PATH;
                }
                TheReal[new PointInt(i, j)] = tile; 
            }
        }
        GameState.TheSeen = TheReal.Clone();
        MyMain.MyDisplay.DrawTileMaps();
        TestPolygon();
    }
    public override void See() {
        GD.Print("VT See");
        List<Point> vertices = Vision.GetVertices(
            TheReal, GameState.PlayerPos
        );
        MyMain.MyDisplay.DrawPolygon(vertices);
        GameState.TheSeen.Clear();
        foreach (KeyValuePair<PointInt, EnumClass> entry in TheReal) {
            GameState.TheSeen[entry.Key] = entry.Value;
        }
    }

    public override void PlayerMove(int x, int y) {
        var (pX, pY) = GameState.PlayerPos;
        pX += x;
        pY += y;
        if (((Tile) TheReal[pX, pY]).DoesBlock()) {
            GD.Print("Hitting a doesBlock.");
        } else {
            GameState.PlayerPos = new PointInt(pX, pY);
            See();
            MyMain.MyDisplay.DrawTileMaps();
        }
    }
    protected void TestPolygon() {
        List<Point> vertices = new List<Point>();
        vertices.Add(new Point(1, 2));
        vertices.Add(new Point(-1, 2));
        vertices.Add(new Point(0, 0));
        MyMain.MyDisplay.DrawPolygon(vertices);
    }
}
