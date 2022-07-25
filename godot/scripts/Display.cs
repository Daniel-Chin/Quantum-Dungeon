using Godot;
using System;
using System.Collections.Generic;

class Display {
    public TileMap SeenTileMap;
    public TileMap OldTileMap;
    public TileMap FolkTileMap;
    public Display(Main main) {
        SeenTileMap = main.GetNode<TileMap>("SeenTileMap");
        OldTileMap  = main.GetNode<TileMap>("OldTileMap");
        FolkTileMap = main.GetNode<TileMap>("FolkTileMap");
        OldTileMap.Modulate = new Color(100, 100, 100);
    }
    public void Draw() {
        DrawTileMap(SeenTileMap, GameState.TheSeen.DownCast());
        DrawTileMap( OldTileMap, GameState.TheOld .DownCast());
        DrawTileMap(FolkTileMap, GameState.TheFolk.DownCast());
        var (x, y) = GameState.PlayerPos;
        FolkTileMap.SetCell(x, y, (int) Folk.PLAYER.Id);
    }
    private void DrawTileMap(
        TileMap tileMap, 
        Map<EnumClass> map
    ) {
        tileMap.Clear();
        foreach (
            KeyValuePair<Tuple<int, int>, EnumClass> 
            cell in map
        ) {
            var (x, y) = cell.Key;
            tileMap.SetCell(x, y, (int) cell.Value.Id);
        }
    }
}
