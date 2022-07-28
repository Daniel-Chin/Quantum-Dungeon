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
        OldTileMap.Modulate = new Color(0, 50, 100);
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
    public static Vector2 Coord2Pixel(Point coord) {
        return new Vector2(
            (float) (coord.X * C.CELL_SIZE), 
            (float) (coord.Y * C.CELL_SIZE)
        );
    }
    public void DrawPolygon(List<Point> vertices) {
        // GD.PrintS("Polygon");
        // double last = - Math.PI;
        // foreach (Point p in vertices) {
        //     double next = Math.Atan2(
        //         p.Y - GameState.PlayerPos.Offset05().Y, 
        //         p.X - GameState.PlayerPos.Offset05().X 
        //     );
        //     GD.PrintS(" ", p, next - last);
        //     last = next;
        // }
        // GD.PrintS();
        Polygon.Polygon = vertices.Select(Coord2Pixel).ToArray();
    }
}
