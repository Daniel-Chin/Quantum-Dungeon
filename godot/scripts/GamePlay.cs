using Godot;
using System;
using System.Collections.Generic;
public class GamePlay {
    protected Main MyMain;
    public GamePlay(Main main) {
        MyMain = main;
    }
    public virtual void Reset() {
        GameState.Reset();
        See();
    }
    public virtual void PlayerMove(int x, int y) {
        Map theSeen = GameState.TheSeen;
        var (pX, pY) = GameState.PlayerPos;
        pX += x;
        pY += y;
        Tile target = (Tile) theSeen[pX, pY];
        if (target.DoesBlockVision()) {
            if (target == Tile.DOOR_SHUT) {
                theSeen[pX, pY] = Tile.DOOR_OPEN;
            } else {
                GD.Print("Movement in vain.");
                return;
            }
        }
        if (theSeen[GameState.PlayerPos] == Tile.DOOR_SHUT) {
            theSeen[GameState.PlayerPos] = Tile.DOOR_OPEN;
        }
        GameState.PlayerPos = new PointInt(pX, pY);
        See();
    }
    public virtual void Process(float dt) {}
    public virtual void See() {
        Map draft = GameState.TheSeen.Clone();
        Point eyePos = GameState.PlayerPos.Offset05();
        List<Point> vertices;
        while (true) {
            vertices = Vision.GetVertices(
                draft, eyePos
            );
            PosNegMatrix seenPNM = Raster.Main(vertices, eyePos);
            List<PointInt> knownUnknowns = SeeUnknowns(
                draft, seenPNM
            );
            if (knownUnknowns.Count == 0) {
                CleanUnseen(draft, seenPNM);
                break;
            }
            foreach (PointInt origin in knownUnknowns) {
                Generate(origin, draft, GameState.PlayerPos);
            }
        }
        GameState.TheSeen = draft;
        MyMain.MyDisplay.DrawTileMaps();
        MyMain.MyDisplay.DrawPolygon(vertices);
    }
    protected List<PointInt> SeeUnknowns(Map map, PosNegMatrix isSeen) {
        List<PointInt> result = new List<PointInt>();
        for (int x = isSeen.XStart; x < isSeen.XEnd; x ++) {
            for (int y = isSeen.YStart; y < isSeen.YEnd; y ++) {
                if (isSeen[x, y] && map[x, y] == Tile.UNKNOWN) {
                    result.Add(new PointInt(x, y));
                }
            }
        }
        return result;
    }
    protected void CleanUnseen(Map map, PosNegMatrix isSeen) {
        for (int x = isSeen.XStart; x < isSeen.XEnd; x ++) {
            for (int y = isSeen.YStart; y < isSeen.YEnd; y ++) {
                if (! isSeen[x, y]) {
                    map[x, y] = Tile.UNKNOWN;
                }
            }
        }
    }
    protected void Generate(
        PointInt origin, Map draft, PointInt playerPos
    ) {
        if (draft[origin] != Tile.UNKNOWN) return;
    }
}
