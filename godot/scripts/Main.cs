using Godot;

public class Main : Node2D {
    protected GamePlay Game;
    public Display MyDisplay;
    public override void _Ready() {
        MyDisplay = new Display(this);
        Game = new VisionTester(this);

        Reset();
    }

    public void Reset() {
        Game.Reset();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey) {
            if (
                eventKey.Pressed && 
                eventKey.Scancode == (int) KeyList.K
            ) {
                ((VisionTester) Game).OnK();
            }
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
