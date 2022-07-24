using System;

public class Global {
    public static Random random;
    static Global() {
        random = new Random();
    }
}
