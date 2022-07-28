using Godot;
using System;
using System.Collections.Generic;
class Zip<T0, T1> {
    public static IEnumerable<Tuple<T0, T1>> Main(
        IEnumerable<T0> e0, 
        IEnumerable<T1> e1
    ) {
        IEnumerator<T0> et0 = e0.GetEnumerator();
        IEnumerator<T1> et1 = e1.GetEnumerator();
        while (true) {
            if (! et0.MoveNext()) yield break;
            if (! et1.MoveNext()) yield break;
            yield return new Tuple<T0, T1>(
                et0.Current, 
                et1.Current
            );
        }
    }
}
