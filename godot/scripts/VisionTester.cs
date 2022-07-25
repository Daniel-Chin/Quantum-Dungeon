using System;
using System.Collections.Generic;

public class VisionTester {
    Map<Tile> TheReal;
    public VisionTester() {
        TheReal = new Map<Tile>();
        for (int i = -10; i < 10; i ++) {
            for (int j = -10; j < 10; j ++) {
                Tile tile;
                if (Global.random.Next() % 2 == 0) {
                    tile = Tile.PATH;
                } else {
                    tile = Tile.WALL;
                }
                TheReal[Tuple.Create(i, j)] = tile; 
            }
        }
    }
    public void See(
        Map<Tile> theSeen, 
        Tuple<int, int> playerPos
    ) {
        theSeen.Clear();
        Dictionary<
            Tuple<int, int>, bool
        > isSeen = Vision.See(playerPos, TheReal);
        foreach (KeyValuePair<Tuple<int, int>, bool> entry in isSeen) {
            Vision.Assert(entry.Value);
            theSeen[entry.Key] = TheReal[entry.Key];
        }
    }
}
