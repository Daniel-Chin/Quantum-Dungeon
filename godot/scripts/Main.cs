using Godot;
using System;
using System.Collections.Generic;

public class Main : Node2D {
    Dictionary<Tuple<int, int>, int> TheSeen;
    Tuple<int, int> PlayerPos;

    TileMap MyTileMap;
    
    public override void _Ready() {
        TheSeen = new Dictionary<Tuple<int, int>, int> ();
        PlayerPos = Tuple.Create(0, 0);
        MyTileMap = GetNode<TileMap> ("MyTileMap");
    }
}
