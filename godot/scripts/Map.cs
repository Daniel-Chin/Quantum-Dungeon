using System;
using System.Collections.Generic;
using System.Text;

public class Map : Dictionary<PointInt, EnumClass> {
    public override string ToString() {
        int xStart = int.MaxValue; 
        int yStart = int.MaxValue; 
        int xEnd   = int.MinValue; 
        int yEnd   = int.MinValue;    
        foreach (PointInt p in this.Keys) {
            xStart = Math.Min(xStart, p.IntX);
            yStart = Math.Min(yStart, p.IntY);
            xEnd   = Math.Max(xEnd  , p.IntX);
            yEnd   = Math.Max(yEnd  , p.IntY);
        }
        xEnd ++;
        yEnd ++;
        int[,] buffer = new int[xEnd - xStart, yEnd - yStart];
        StringBuilder sB = new StringBuilder();
        sB.Append("Map: ");
        for (int y = yStart; y < yEnd; y ++) {
            sB.Append("\n");
            for (int x = xStart; x < xEnd; x ++) {
                sB.Append(" ");
                sB.Append(this[x, y].Id);
            }
        }
        return sB.ToString();
    }
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
    public void WrapWithUnknown() {
        HashSet<PointInt> todo = new HashSet<PointInt>();
        foreach (KeyValuePair<PointInt, EnumClass> cell in this) {
            PointInt p = cell.Key;
            Tile tile = (Tile) cell.Value;
            if (! tile.DoesBlockVision()) {
                PointInt t;
                t = p + new PointInt(0, 1);
                if (! this.ContainsKey(t)) todo.Add(t);
                t = p + new PointInt(1, 0);
                if (! this.ContainsKey(t)) todo.Add(t);
                t = p + new PointInt(0, -1);
                if (! this.ContainsKey(t)) todo.Add(t);
                t = p + new PointInt(-1, 0);
                if (! this.ContainsKey(t)) todo.Add(t);
            }
        }
        foreach (PointInt p in todo) {
            this.Add(p, Tile.UNKNOWN);
        }
    }
}
