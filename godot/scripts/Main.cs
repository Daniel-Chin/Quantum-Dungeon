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
    }

    public void Reset() {
        TheSeen = new Dictionary<Tuple<int, int>, int> ();
        TheOld = new Dictionary<Tuple<int, int>, int> ();
        TheFolk = new Dictionary<Tuple<int, int>, int> ();
        PlayerPos = Tuple.Create(0, 0);
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
}
