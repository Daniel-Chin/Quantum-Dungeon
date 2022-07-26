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
                TheReal[Tuple.Create(i, j)] = tile; 
            }
        }
        GameState.TheSeen = TheReal.Clone();
    }
    public void See(
    ) {
        GD.Print("VT See");
        GameState.TheSeen.Clear();
        Dictionary<
            Tuple<int, int>, bool
        > isSeen = Vision.See(GameState.PlayerPos, TheReal);
        foreach (KeyValuePair<Tuple<int, int>, bool> entry in isSeen) {
            Vision.Assert(entry.Value);
            GameState.TheSeen[entry.Key] = TheReal[entry.Key];
        }
    }

    public override void PlayerMove(int x, int y) {
        var (pX, pY) = GameState.PlayerPos;
        pX += x;
        pY += y;
        if (TheReal[pX, pY].DoesBlock()) {
            GD.Print("Hitting a doesBlock.");
        } else {
            GameState.PlayerPos = new Tuple<int, int>(pX, pY);
            See();
            MyMain.Draw();
        }
    }
}
