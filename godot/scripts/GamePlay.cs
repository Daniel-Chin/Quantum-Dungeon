public class GamePlay {
    protected Main MyMain;
    protected Rasterizor Raster;
    public GamePlay(Main main, Rasterizor raster) {
        MyMain = main;
        Raster = raster;
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
