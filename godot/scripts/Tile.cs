public class Tile {
    public static readonly int UNKNOWN = 0;
    public static readonly int PLAYER = 1;
    public static readonly int WALL = 2;
    public static readonly int PATH = 3;
    public static readonly int DOOR_SHUT = 4;
    public static readonly int DOOR_OPEN = 5;

    static Tile() {
    }

    static public bool DoesBlock(int x) {
        if (x == WALL || x == DOOR_SHUT || x == UNKNOWN) {
            return true;
        } else return false;
    }
}
