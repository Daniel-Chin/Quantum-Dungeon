using Godot;

public class Main : Node2D {
    GamePlay Game;
    Display MyDisplay;
    public override void _Ready() {
        Game = new VisionTester(this);
        MyDisplay = new Display(this);

        Reset();
    }

    public void Reset() {
        Game = new VisionTester(this);
        GameState.Reset();

        MyDisplay.Draw();
    }
    public void Draw() {
        MyDisplay.Draw();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey) {
            if (
                eventKey.Pressed && 
                eventKey.Scancode == (int) KeyList.Escape
            ) {
                GetTree().Quit();
            }
        }
        if (@event.IsActionPressed("ui_left")) {
            Game.PlayerMove(-1, 0);
        }
        if (@event.IsActionPressed("ui_right")) {
            Game.PlayerMove(+1, 0);
        }
        if (@event.IsActionPressed("ui_up")) {
            Game.PlayerMove(0, -1);
        }
        if (@event.IsActionPressed("ui_down")) {
            Game.PlayerMove(0, +1);
        }
    }
}
