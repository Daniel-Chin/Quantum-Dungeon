using Godot;
using System;

public class Tile {
    public static readonly int UNKNOWN = 0;
    public static readonly int PLAYER = 1;
    public static readonly int WALL = 2;
    public static readonly int PATH = 3;
    public static readonly int DOOR = 4;

    static Tile() {
    }
}
