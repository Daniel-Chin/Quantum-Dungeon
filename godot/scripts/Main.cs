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
        SeenTileMap = GetNode<TileMap> ("SeenTileMap");
        OldTileMap  = GetNode<TileMap> ("OldTileMap");
        FolkTileMap = GetNode<TileMap> ("FolkTileMap");
        OldTileMap.Modulate = new Color(100, 100, 100);

        Reset();
    }

    public void Reset() {
        TheSeen = new Dictionary<Tuple<int, int>, int> ();
        TheOld = new Dictionary<Tuple<int, int>, int> ();
        TheFolk = new Dictionary<Tuple<int, int>, int> ();
        PlayerPos = Tuple.Create(0, 0);
        int spawn_room_start = -2;
        int spawn_room_end = 3;
        for (int x = spawn_room_start; x < spawn_room_end; x ++) {
            TheSeen[Tuple.Create(
                x, 0         
            )] = Tile.WALL;
            TheSeen[Tuple.Create(
                x, spawn_room_end - 1
            )] = Tile.WALL;
            for (
                int y = spawn_room_start; 
                y < spawn_room_end; y ++
            ) {
                int toPlace;
                if (x == 0 || x == spawn_room_end - 1) {
                    toPlace = Tile.WALL;
                } else {
                    toPlace = Tile.PATH;
                }
                TheSeen[Tuple.Create(x, y)] = toPlace;
            }
        }
        TheSeen[Tuple.Create(0, spawn_room_start)] = Tile.DOOR;

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

    private void See(
        Dictionary<Tuple<int, int>, int> draftMatrix
    ) {

    }
}
