using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
public class PosNegMatrix {
    // Matrix where indices can be negative. 
    public int XStart {
        get; protected set;
    }
    public int XEnd {
        get; protected set;
    }
    public int YStart {
        get; protected set;
    }
    public int YEnd {
        get; protected set;
    }
    protected byte Exponent;
    protected BitArray Data;
    public PosNegMatrix(
        int xStart, 
        int yStart, 
        int xEnd, 
        int yEnd
    ) {
        XStart = xStart;
        YStart = yStart;
        XEnd   = xEnd;
        YEnd   = yEnd;
        Exponent = (byte) Math.Ceiling(Math.Log(
            XEnd - XStart
        ) / Math.Log(2));
        Data = new BitArray((YEnd - YStart) * (1 << Exponent));
    }
    public override String ToString() {
        return $"<PNMat ({XStart}, {YStart})-({XEnd}, {YEnd})>";
    }
    public bool this[int x, int y] {
        get {
            CheckBounds(x, y);
            return Data[(y - YStart) << Exponent ^ (x - XStart)];
        }
        set {
            CheckBounds(x, y);
            Data[(y - YStart) << Exponent ^ (x - XStart)] = value;
        }
    }
    protected void CheckBounds(int x, int y) {
        if (! (
            XStart <= x && x < XEnd && 
            YStart <= y && y < YEnd
        )) {
            throw new IndexOutOfRangeException(
                $"{(x, y)} not in {this}"
            );
        }
    }
    public void InplaceOr(PosNegMatrix that) {
        Data.Or(that.Data);
    }
    public PosNegMatrix Birth() {
        return new PosNegMatrix(
            XStart, 
            YStart, 
            XEnd, 
            YEnd
        );
    }
    public List<PointInt> WhereTrues() {
        List<PointInt> output = new List<PointInt>();
        for (int x = XStart; x < XEnd; x ++) {
            for (int y = YStart; y < YEnd; y ++) {
                if (this[x, y])
                    output.Add(new PointInt(x, y));
            }
        }
        return output;
    }
}
