using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Display {
    public TileMap SeenTileMap;
    public TileMap OldTileMap;
    public TileMap FolkTileMap;
    public Polygon2D Polygon;
    public Display(Main main) {
        SeenTileMap = main.GetNode<TileMap>("SeenTileMap");
        OldTileMap  = main.GetNode<TileMap>("OldTileMap");
        FolkTileMap = main.GetNode<TileMap>("FolkTileMap");
        OldTileMap.Modulate = new Color(100, 100, 100);
        Polygon = main.GetNode<Polygon2D>("Polygon");
    }
    public void DrawTileMaps() {
        DrawOneTileMap(SeenTileMap, GameState.TheSeen);
        DrawOneTileMap( OldTileMap, GameState.TheOld );
        DrawOneTileMap(FolkTileMap, GameState.TheFolk);
        var (x, y) = GameState.PlayerPos;
        FolkTileMap.SetCell(x, y, (int) Folk.PLAYER.Id);
    }
    private void DrawOneTileMap(
        TileMap tileMap, 
        Map map
    ) {
        tileMap.Clear();
        foreach (
            KeyValuePair<PointInt, EnumClass> 
            cell in map
        ) {
            tileMap.SetCell(
                cell.Key.IntX, 
                cell.Key.IntY, 
                (int) cell.Value.Id
            );
        }
    }
    public void DrawPolygon(List<Point> vertices) {
        Polygon.Polygon = vertices.Select(p => new Vector2(
            (float) p.X, (float) p.Y
        )).ToArray();
    }
}
