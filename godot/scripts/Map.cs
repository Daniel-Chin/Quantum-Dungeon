using System;
using System.Collections.Generic;

// public class EnumClassList : List<
//     KeyValuePair<Tuple<int, int>, EnumClass>
// > {}
public class Map<T> : Dictionary<Tuple<int, int>, T> where T : EnumClass {
    public new T this[Tuple<int, int> location] {
        get {
            try {
                return base[location];
            } catch (KeyNotFoundException) {
                return (T) EnumClass.UNKNOWN;
            }
        }
        set {
            base[location] = value;
        }
    }
    public T this[int x, int y] {
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
    public Map<EnumClass> DownCast() {
        Map<EnumClass> map = new Map<EnumClass>();
        foreach (KeyValuePair<Tuple<int, int>, T> cell in this) {
            map.Add(cell.Key, cell.Value as EnumClass);
        }
        return map;
    }
    public Map<T> Clone() {
        Map<T> clone = new Map<T>();
        foreach (KeyValuePair<Tuple<int, int>, T> cell in this) {
            clone.Add(cell.Key, cell.Value);
        }
        return clone;
    }
}
