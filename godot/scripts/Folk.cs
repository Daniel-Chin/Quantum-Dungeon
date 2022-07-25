public class Folk : EnumClass {
    public static readonly Folk PLAYER    = new Folk(1);

    protected Folk(uint id): base(id) {
    }
}
