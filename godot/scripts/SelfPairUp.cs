// In a circular way.

using Godot;
using System;
using System.Collections.Generic;
class SelfPairUp<T> {
    public static IEnumerable<Tuple<T, T>> Main(IEnumerable<T> e) {
        T first = default(T);
        T prev = default(T);
        bool isFirst = true;
        foreach (T x in e) {
            if (isFirst) {
                first = x;
            } else {
                yield return new Tuple<T, T>(prev, x);
            }
            prev = x;
        }
        yield return new Tuple<T, T>(prev, first);
    }
}
