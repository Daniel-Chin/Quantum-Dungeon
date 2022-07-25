public class GamePlay {
    protected Main MyMain;
    public GamePlay(Main main) {
        MyMain = main;
    }
    public virtual void Reset() {
        GameState.Reset();
    }
    public virtual void PlayerMove(int x, int y) {
        
    }
}
