using Godot;
using System;
using System.Collections.Generic;

public class Main : Node2D {
    Dictionary<Tuple<int, int>, int> TheSeen;
    Dictionary<Tuple<int, int>, int> TheOld;
    Dictionary<Tuple<int, int>, int> TheFolk;
    Tuple<int, int> PlayerPos;

    TileMap SeenTileMap;
    TileMap OldTileMap;
    TileMap FolkTileMap;

    public override void _Ready() {
        SeenTileMap = GetNode<TileMap>("SeenTileMap");
        OldTileMap  = GetNode<TileMap>("OldTileMap");
        FolkTileMap = GetNode<TileMap>("FolkTileMap");
        OldTileMap.Modulate = new Color(100, 100, 100);

        Reset();
    }

    public void Reset() {
        TheSeen = new Dictionary<Tuple<int, int>, int>();
        TheOld = new Dictionary<Tuple<int, int>, int>();
        TheFolk = new Dictionary<Tuple<int, int>, int>();
        PlayerPos = Tuple.Create(0, 0);
        int spawn_room_start = -2;
        int spawn_room_end = 3;
        for (int x = spawn_room_start; x < spawn_room_end; x ++) {
            for (
                int y = spawn_room_start; 
                y < spawn_room_end; y ++
            ) {
                int toPlace;
                if (
                    x == spawn_room_start || 
                    x == spawn_room_end - 1
                ) {
                    toPlace = Tile.WALL;
                } else {
                    toPlace = Tile.PATH;
                }
                TheSeen[Tuple.Create(x, y)] = toPlace;
            }
            TheSeen[Tuple.Create(
                x, spawn_room_start
            )] = Tile.WALL;
            TheSeen[Tuple.Create(
                x, spawn_room_end - 1
            )] = Tile.WALL;
        }
        TheSeen[Tuple.Create(
            0, spawn_room_start
        )] = Tile.DOOR_SHUT;

        Draw();
    }

    public void Draw() {
        DrawTileMap(SeenTileMap, TheSeen);
        DrawTileMap(OldTileMap, TheOld);
        DrawTileMap(FolkTileMap, TheFolk);
        var (x, y) = PlayerPos;
        FolkTileMap.SetCell(x, y, Tile.PLAYER);
    }
    private void DrawTileMap(
        TileMap tileMap, 
        Dictionary<Tuple<int, int>, int> matrix
    ) {
        tileMap.Clear();
        foreach (KeyValuePair<
            Tuple<int, int>, int
        > cell in matrix) {
            var (x, y) = cell.Key;
            tileMap.SetCell(x, y, cell.Value);
        }
    }

    private class VisionTester {
        Dictionary<Tuple<int, int>, int> TheReal;
        public VisionTester() {
            TheReal = new Dictionary<Tuple<int, int>, int>();
            for (int i = -10; i < 10; i ++) {
                for (int j = -10; j < 10; j ++) {
                    int tile;
                    if (Global.random.Next() % 2 == 0) {
                        tile = Tile.PATH;
                    } else {
                        tile = Tile.WALL;
                    }
                    TheReal[Tuple.Create(i, j)] = tile; 
                }
            }
        }
        public void See(
            Dictionary<Tuple<int, int>, int> theSeen, 
            Tuple<int, int> playerPos
        ) {
            theSeen.Clear();
            Dictionary<
                Tuple<int, int>, bool
            > isSeen = Vision.See(playerPos, TheReal);
            foreach (KeyValuePair<Tuple<int, int>, bool> entry in isSeen) {
                Vision.Assert(entry.Value);
                theSeen[entry.Key] = TheReal[entry.Key];
            }
        }
    }
}
