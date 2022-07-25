public class Tile : EnumClass {
    public static readonly Tile UNKNOWN   = new Tile(0);
    public static readonly Tile WALL      = new Tile(2);
    public static readonly Tile PATH      = new Tile(3);
    public static readonly Tile DOOR_SHUT = new Tile(4);
    public static readonly Tile DOOR_OPEN = new Tile(5);

    protected Tile(int id): base(id) {
    }

    public bool DoesBlock() => (
        this == WALL || 
        this == DOOR_SHUT || 
        this == UNKNOWN
    );
}
