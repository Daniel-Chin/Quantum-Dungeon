using System;
using System.Collections.Generic;

public class Map : Dictionary<Tuple<int, int>, Tile> {
    public new Tile this[Tuple<int, int> location] {
        get {
            try {
                return base[location];
            } catch (KeyNotFoundException) {
                return Tile.UNKNOWN;
            }
        }
        set {
            base[location] = value;
        }
    }
}
