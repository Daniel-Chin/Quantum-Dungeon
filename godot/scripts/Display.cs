using Godot;
using System;
using System.Collections.Generic;

class Display {
    public TileMap SeenTileMap;
    public TileMap OldTileMap;
    public TileMap FolkTileMap;
    public Display(Node2D main) {
        SeenTileMap = main.GetNode<TileMap>("SeenTileMap");
        OldTileMap  = main.GetNode<TileMap>("OldTileMap");
        FolkTileMap = main.GetNode<TileMap>("FolkTileMap");
        OldTileMap.Modulate = new Color(100, 100, 100);
    }
    public void Draw() {
        DrawTileMap(SeenTileMap, GameState.TheSeen.ToEnumClassList());
        DrawTileMap( OldTileMap, GameState.TheOld .ToEnumClassList());
        DrawTileMap(FolkTileMap, GameState.TheFolk.ToEnumClassList());
        var (x, y) = GameState.PlayerPos;
        FolkTileMap.SetCell(x, y, (int) Folk.PLAYER.Id);
    }
    private void DrawTileMap(
        TileMap tileMap, 
        EnumClassList enumClassList
    ) {
        tileMap.Clear();
        foreach (
            KeyValuePair<Tuple<int, int>, EnumClass> 
            cell in enumClassList
        ) {
            var (x, y) = cell.Key;
            tileMap.SetCell(x, y, (int) cell.Value.Id);
        }
    }
}
