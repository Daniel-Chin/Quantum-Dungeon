using Godot;
using System;
using System.Collections.Generic;

public class VisionTester : GamePlay {
    Map TheReal;
    DebugCanvas DebugCanvas;
    public VisionTester(
        Main main
    ): base(main) {
        DebugCanvas = main.GetNode<DebugCanvas>("DebugCanvas");
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
                if (
                    i == -10 ||
                    j == -10 ||
                    i == 9   ||
                    j == 9  
                )
                    tile = Tile.WALL;
                TheReal[new PointInt(i, j)] = tile; 
            }
        }
        GameState.TheSeen = TheReal.Clone();
        MyMain.MyDisplay.DrawTileMaps();
        TestPolygon();
    }
    public override void See() {
        // GD.Print("VT See");
        List<Point> vertices = Vision.GetVertices(
            TheReal, GameState.PlayerPos
        );
        MyMain.MyDisplay.DrawPolygon(vertices);
        DebugCanvas.Update();

        GameState.TheSeen.Clear();
        PosNegMatrix IsSeen = Raster.RasterOnlyEdges(vertices);
        foreach (PointInt p in IsSeen.WhereTrues()) {
            GameState.TheSeen[p] = TheReal[p];
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
        vertices.Add(new Point(0, 0));
        vertices.Add(new Point(-1, 2));
        MyMain.MyDisplay.DrawPolygon(vertices);
    }
    public void OnK() {
        // Vision.DEBUG_I ++;
        // See();
        // DebugCanvas.Self.Update();
    }
    public override void Process(float dt) {
    }
    void DebugRasterLineSeg() {
        GameState.TheSeen.Clear();
        PosNegMatrix IsSeen = new PosNegMatrix(-99, -99, 99, 99);
        Vector2 v = MyMain.MyDisplay.SeenTileMap.GetLocalMousePosition();
        // v = MyMain.MyDisplay.SeenTileMap.WorldToMap(
            // MyMain.MyDisplay.SeenTileMap.ToLocal(
                // v
            // )
        // );
        v = v / C.CELL_SIZE;
        // GD.PrintS(v);
        Raster.LineSeg(new LineSegment(
            new Point(0.5, 0.5), 
            new Point(v.x, v.y)
        ), IsSeen);
        foreach (PointInt p in IsSeen.WhereTrues()) {
            GameState.TheSeen[p] = TheReal[p];
        }
        MyMain.MyDisplay.DrawTileMaps();
    }
}
