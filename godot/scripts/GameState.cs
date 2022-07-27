using System;

class GameState {
    public static Map TheSeen;
    public static Map TheOld;
    public static Map TheFolk;
    public static PointInt PlayerPos;
    
    public static void Reset() {
        TheSeen = new Map();
        TheOld  = new Map();
        TheFolk = new Map();
        PlayerPos = new PointInt(0, 0);

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
                TheSeen[new PointInt(x, y)] = toPlace;
            }
            TheSeen[new PointInt(
                x, spawn_room_start
            )] = Tile.WALL;
            TheSeen[new PointInt(
                x, spawn_room_end - 1
            )] = Tile.WALL;
        }
        TheSeen[new PointInt(
            0, spawn_room_start
        )] = Tile.DOOR_SHUT;
    }
}
