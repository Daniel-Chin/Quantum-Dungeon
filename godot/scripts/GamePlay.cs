using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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
        if (theSeen[GameState.PlayerPos] == Tile.DOOR_OPEN) {
            theSeen[GameState.PlayerPos] = Tile.DOOR_SHUT;
        }
        GameState.PlayerPos = new PointInt(pX, pY);
        See();
    }
    public virtual void Process(float dt) {}
    public virtual void See() {
        Map draft = GameState.TheSeen.Clone();
        GD.PrintS(draft);
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
            // prioritize room generate, to avoid collision
            Generate(GameState.PlayerPos + new PointInt(
                1, 0
            ), draft, GameState.PlayerPos);
            Generate(GameState.PlayerPos + new PointInt(
                0, 1
            ), draft, GameState.PlayerPos);
            Generate(GameState.PlayerPos + new PointInt(
                -1, 0
            ), draft, GameState.PlayerPos);
            Generate(GameState.PlayerPos + new PointInt(
                0, -1
            ), draft, GameState.PlayerPos);
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
        List<PointInt> todo = new List<PointInt>();
        foreach (PointInt p in map.Keys) {
            bool toClean = false;
            try {
                toClean = ! isSeen[p.IntX, p.IntY];
            } catch (IndexOutOfRangeException) {
                toClean = true;
            }
            if (toClean) {
                todo.Add(p);
            }
        }
        todo.ForEach(p => map.Remove(p));
    }
    protected void Generate(
        PointInt origin, Map draft, PointInt playerPos
    ) {
        if (draft[origin] != Tile.UNKNOWN) return;
        PointInt playerToOrigin = origin - playerPos;
        if (
            Math.Abs(playerToOrigin.IntX) + 
            Math.Abs(playerToOrigin.IntY)
            == 1 && draft[playerPos] == Tile.DOOR_OPEN
        ) {
            // generate room
            PointInt baseI = playerToOrigin;
            PointInt baseJ = playerToOrigin.Rotate90();
            int radius = Global.random.Next() % 5 + 1;
            for (int i = 0; i <= 2 * radius; i ++) {
                for (int j = - radius; j <= radius; j ++) {
                    PointInt target = origin + i * baseI + j * baseJ;
                    new Assert(draft[target] == Tile.UNKNOWN);
                    draft[target] = Tile.PATH;
                }
            }
            for (int i = 0; i <= 2 * radius; i ++) {
                for (int j = - radius; j <= radius; j += 2 * radius) {
                    PointInt target = origin + i * baseI + j * baseJ;
                    if (i % 2 == 0 && Global.random.Next() % 8 == 0) {
                        draft[target] = Tile.DOOR_SHUT;
                    } else {
                        draft[target] = Tile.WALL;
                    }
                }
            }
            for (int j = - radius; j <= radius; j ++) {
                PointInt target = origin + 2 * radius * baseI + j * baseJ;
                if (j % 2 == 0 && Global.random.Next() % 8 == 0) {
                    draft[target] = Tile.DOOR_SHUT;
                } else {
                    draft[target] = Tile.WALL;
                }
            }
        } else {
            // generate path or maybe corner wall
            draft[origin] = Tile.WALL;
        }
    }
}
