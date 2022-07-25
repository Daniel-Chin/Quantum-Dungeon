using Godot;
using System;
using System.Collections.Generic;

public class Main : Node2D {
    GamePlay Game;
    Display MyDisplay;
    public override void _Ready() {
        Game = new GamePlay();
        MyDisplay = new Display(this);

        Reset();
    }

    public void Reset() {
        Game = new GamePlay();
        GameState.Reset();

        MyDisplay.Draw();
    }
}
