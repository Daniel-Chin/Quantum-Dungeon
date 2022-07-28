using System;
using System.Collections.Generic;

public class Map : Dictionary<PointInt, EnumClass> {
    public new EnumClass this[PointInt location] {
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
    public EnumClass this[int x, int y] {
        get {
            return this[new PointInt(x, y)];
        }
        set {
            this[new PointInt(x, y)] = value;
        }
    }
    public Map Clone() {
        Map clone = new Map();
        foreach (KeyValuePair<PointInt, EnumClass> cell in this) {
            clone.Add(cell.Key, cell.Value);
        }
        return clone;
    }
}
