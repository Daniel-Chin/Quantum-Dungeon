using Godot;
using System;
using System.Collections.Generic;

public class Main : Node2D {
    Map<Tile> TheSeen;
    Map<Tile> TheOld;
    Map<Folk> TheFolk;
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
        TheSeen = new Map<Tile>();
        TheOld = new Map<Tile>();
        TheFolk = new Map<Folk>();
        PlayerPos = Tuple.Create(0, 0);
        int spawn_room_start = -2;
        int spawn_room_end = 3;
        for (int x = spawn_room_start; x < spawn_room_end; x ++) {
            for (
                int y = spawn_room_start; 
                y < spawn_room_end; y ++
            ) {
                Tile toPlace;
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
        DrawTileMap(SeenTileMap, TheSeen.ToEnumClassList());
        DrawTileMap( OldTileMap, TheOld .ToEnumClassList());
        DrawTileMap(FolkTileMap, TheFolk.ToEnumClassList());
        var (x, y) = PlayerPos;
        FolkTileMap.SetCell(x, y, (int) Folk.PLAYER.Id);
    }
    private void DrawTileMap(
        TileMap tileMap, 
        EnumClassList matrix
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
        Map TheReal;
        public VisionTester() {
            TheReal = new Map();
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
            Map theSeen, 
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
