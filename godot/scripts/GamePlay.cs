public class GamePlay {
    protected Main MyMain;
    public GamePlay(Main main) {
        MyMain = main;
    }
    public virtual void Reset() {
        GameState.Reset();
        MyMain.MyDisplay.DrawTileMaps();
        See();
    }
    public virtual void See() {
    }
    public virtual void PlayerMove(int x, int y) {
        
    }
}
