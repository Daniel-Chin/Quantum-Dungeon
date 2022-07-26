using System;
using System.Collections.Generic;

// public class EnumClassList : List<
//     KeyValuePair<Tuple<int, int>, EnumClass>
// > {}
public class Map : Dictionary<Point, EnumClass> {
    public new EnumClass this[Point location] {
        get {
            try {
                return base[location];
            } catch (KeyNotFoundException) {
                return EnumClass.UNKNOWN;
            }
        }
        set {
            base[location] = value;
        }
    }
    public EnumClass this[int x, int y] {
        get {
            return this[new Point(x, y)];
        }
        set {
            this[new Point(x, y)] = value;
        }
    }
    // public EnumClassList ToEnumClassList() {
    //     return (EnumClassList) this.ToList().Select(
    //         x => new KeyValuePair<Tuple<int, int>, EnumClass>(
    //             x.Key, x.Value as EnumClass
    //         )
    //     ).ToList();
    // }
    public Map Clone() {
        Map clone = new Map();
        foreach (KeyValuePair<Point, EnumClass> cell in this) {
            clone.Add(cell.Key, cell.Value);
        }
        return clone;
    }
}
