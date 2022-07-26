using System;

class GameState {
    public static Map TheSeen;
    public static Map TheOld;
    public static Map TheFolk;
    public static Tuple<int, int> PlayerPos;
    
    public static void Reset() {
        TheSeen = new Map();
        TheOld  = new Map();
        TheFolk = new Map();
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
    }
}
