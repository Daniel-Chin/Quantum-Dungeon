using System;
using System.Collections.Generic;

// public class EnumClassList : List<
//     KeyValuePair<Tuple<int, int>, EnumClass>
// > {}
public class Map : Dictionary<Tuple<int, int>, EnumClass> {
    public new EnumClass this[Tuple<int, int> location] {
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
            return this[new Tuple<int, int>(x, y)];
        }
        set {
            this[new Tuple<int, int>(x, y)] = value;
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
        foreach (KeyValuePair<Tuple<int, int>, EnumClass> cell in this) {
            clone.Add(cell.Key, cell.Value);
        }
        return clone;
    }
}
